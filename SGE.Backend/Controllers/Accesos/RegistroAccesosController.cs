using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGE.Backend.Data;
using SGE.Backend.DTOs.Accesos;
using SGE.Backend.Models;
using System.Security.Claims;

namespace SGE.Backend.Controllers.Accesos
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protección por Token JWT y RBAC (RNF-08)
    public class RegistroAccesosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegistroAccesosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// RF-07 / RNF-01: Verificación de Estudiante y Equipos en Portería.
        /// Despliega información académica, fotos de perfil, equipos y accesorios en < 2.0s.
        /// </summary>
        [HttpGet("verificar/{numeroDocumento}")]
        [Authorize(Roles = "SuperAdmin,Guardia")]
        public async Task<IActionResult> VerificarEstudiante(string numeroDocumento)
        {
            // Búsqueda sobre el DbSet Estudiantes de ApplicationDbContext
            var estudiante = await _context.Estudiantes
                .Include(e => e.Carrera) // Navegación 'Carrera' de tu Modelo Estudiante.cs
                .Include(e => e.Equipos)
                .AsNoTracking() // Optimización para cumplir RNF-01 (< 2.0 segundos)
                .FirstOrDefaultAsync(e => e.NumeroDocumento == numeroDocumento);

            if (estudiante == null)
            {
                return NotFound(new { mensaje = "El número de documento no corresponde a un estudiante registrado." });
            }

            if (estudiante.Estado != "Activo")
            {
                return BadRequest(new { mensaje = "ACCESO DENEGADO: El estudiante se encuentra en estado INACTIVO." });
            }

            var respuesta = new
            {
                estudiante.IdEstudiante,
                estudiante.NumeroDocumento,
                NombreCompleto = $"{estudiante.Nombres} {estudiante.Apellidos}",
                Carrera = estudiante.Carrera?.NombreCarrera ?? "N/A",
                estudiante.FotoPerfilUrl,
                estudiante.Estado,
                EquiposRegistrados = estudiante.Equipos.Select(eq => new
                {
                    eq.IdEquipo,
                    eq.Serial,
                    eq.FotoEquipoUrl,
                    eq.Estado
                })
            };

            return Ok(respuesta);
        }

        /// <summary>
        /// RF-08 / RNF-02: Registrar tiempos de acceso (Entrada / Salida).
        /// Guarda la marca de tiempo exacta, la portería y la identidad del guardia en turno.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Guardia")]
        public async Task<IActionResult> RegistrarAcceso([FromBody] RegistroAccesoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Obtener la identidad del Guardia autenticado desde los Claims del JWT (RNF-08)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("id_usuario")?.Value;

            if (!int.TryParse(userIdClaim, out int idGuardia))
            {
                return Unauthorized(new { mensaje = "No se pudo identificar al guardia en sesión." });
            }

            // Validar que el estudiante exista y esté activo
            var estudiante = await _context.Estudiantes.FindAsync(dto.IdEstudiante);
            if (estudiante == null)
            {
                return NotFound(new { mensaje = "Estudiante no encontrado." });
            }

            if (estudiante.Estado != "Activo")
            {
                return BadRequest(new { mensaje = "No se puede registrar acceso a un estudiante INACTIVO." });
            }

            // Instanciar la entidad usando la tabla 'registro_accesos'
            var nuevoAcceso = new RegistroAcceso
            {
                IdEstudiante = dto.IdEstudiante,
                IdEquipo = dto.IdEquipo,
                IdUsuarioGuardia = idGuardia,
                IdPorteria = dto.IdPorteria,
                TipoMovimiento = dto.TipoMovimiento,
                FechaHora = DateTime.Now // Timestamp exacto de portería
            };

            // Usamos tu DbSet exacto: RegistrosAccesos
            await _context.RegistrosAccesos.AddAsync(nuevoAcceso);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = $"Registro de {dto.TipoMovimiento} exitoso.",
                nuevoAcceso.IdRegistro,
                nuevoAcceso.FechaHora
            });
        }
    }
}