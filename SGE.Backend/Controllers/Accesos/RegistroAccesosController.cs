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
    [Authorize] // Protección por Token JWT
    public class RegistroAccesosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegistroAccesosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// RF-07 / RNF-01: Verificación de Estudiante y Equipos en Portería.
        /// Accesible por SuperAdmin, Administrador y Guardia.
        /// </summary>
        [HttpGet("verificar/{numeroDocumento}")]
        [Authorize(Roles = "SuperAdmin,Administrador,Guardia")]
        public async Task<IActionResult> VerificarEstudiante(string numeroDocumento)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.Carrera)
                    .ThenInclude(c => c.Facultad)
                .Include(e => e.Equipos)
                    .ThenInclude(eq => eq.Marca)
                .Include(e => e.Equipos)
                    .ThenInclude(eq => eq.Tipo)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.NumeroDocumento == numeroDocumento);

            if (estudiante == null)
            {
                return NotFound(new { mensaje = "El número de documento no corresponde a un estudiante registrado." });
            }

            if (estudiante.Estado != "Activo")
            {
                return BadRequest(new { mensaje = "ACCESO DENEGADO: El estudiante se encuentra en estado INACTIVO." });
            }

            // Consultar el ÚLTIMO REGISTRO DE ACCESO
            var ultimoAcceso = await _context.RegistrosAccesos
                .Where(ra => ra.IdEstudiante == estudiante.IdEstudiante)
                .OrderByDescending(ra => ra.FechaHora)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            bool estaDentro = ultimoAcceso != null && ultimoAcceso.TipoMovimiento == "Entrada";
            DateTime? ultimoIngreso = (ultimoAcceso != null && ultimoAcceso.TipoMovimiento == "Entrada")
                ? ultimoAcceso.FechaHora
                : null;

            var equipoPrincipal = estudiante.Equipos.FirstOrDefault();

            var respuesta = new
            {
                estudiante.IdEstudiante,
                estudiante.NumeroDocumento,
                NombreCompleto = $"{estudiante.Nombres} {estudiante.Apellidos}",
                Carrera = estudiante.Carrera?.NombreCarrera ?? "N/A",
                Facultad = estudiante.Carrera?.Facultad?.NombreFacultad ?? "N/A",
                Correo = estudiante.CorreoElectronico ?? "N/A",
                Telefono = estudiante.Telefono ?? "N/A",
                estudiante.FotoPerfilUrl,
                estudiante.Estado,

                EstaDentro = estaDentro,
                UltimoIngreso = ultimoIngreso,

                TieneEquipo = equipoPrincipal != null,
                Equipo = equipoPrincipal == null ? null : new
                {
                    equipoPrincipal.IdEquipo,
                    equipoPrincipal.Serial,
                    Marca = equipoPrincipal.Marca?.NombreMarca ?? "Generico",
                    Tipo = equipoPrincipal.Tipo?.NombreTipo ?? "Portátil",
                    equipoPrincipal.FotoEquipoUrl,
                    equipoPrincipal.FotoAccesoriosUrl
                }
            };

            return Ok(respuesta);
        }

        /// <summary>
        /// RF-08 / RNF-02: Registrar tiempos de acceso (Entrada / Salida).
        /// Accesible por SuperAdmin, Administrador y Guardia.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Administrador,Guardia")]
        public async Task<IActionResult> RegistrarAcceso([FromBody] RegistroAccesoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Extraer el ID del usuario en sesión (sea SuperAdmin, Admin o Guardia)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("id_usuario")?.Value
                           ?? User.FindFirst("id")?.Value
                           ?? User.FindFirst("sub")?.Value;

            int idUsuario = 1; // Respaldo para pruebas
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int parsedId))
            {
                idUsuario = parsedId;
            }

            var estudiante = await _context.Estudiantes.FindAsync(dto.IdEstudiante);
            if (estudiante == null)
            {
                return NotFound(new { mensaje = "Estudiante no encontrado." });
            }

            if (estudiante.Estado != "Activo")
            {
                return BadRequest(new { mensaje = "No se puede registrar acceso a un estudiante INACTIVO." });
            }

            var nuevoAcceso = new RegistroAcceso
            {
                IdEstudiante = dto.IdEstudiante,
                IdEquipo = dto.IdEquipo,
                IdUsuarioGuardia = idUsuario,
                IdPorteria = dto.IdPorteria,
                TipoMovimiento = dto.TipoMovimiento,
                FechaHora = DateTime.Now
            };

            await _context.RegistrosAccesos.AddAsync(nuevoAcceso);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = $"Registro de {dto.TipoMovimiento} exitoso.",
                nuevoAcceso.IdRegistro,
                nuevoAcceso.FechaHora
            });
        }

        /// <summary>
        /// Obtiene el historial de movimientos del estudiante en portería.
        /// Accesible por SuperAdmin, Administrador y Guardia.
        /// </summary>
        [HttpGet("historial/{numeroDocumento}")]
        [Authorize(Roles = "SuperAdmin,Administrador,Guardia")]
        public async Task<IActionResult> ObtenerHistorialAcceso(string numeroDocumento, [FromQuery] DateTime? fecha, [FromQuery] string? serial)
        {
            var estudiante = await _context.Estudiantes
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.NumeroDocumento == numeroDocumento);

            if (estudiante == null)
            {
                return Ok(new List<RegistroAccesoReadDto>());
            }

            var query = _context.RegistrosAccesos
                .Include(r => r.Equipo)
                .Include(r => r.Guardia)
                .Where(r => r.IdEstudiante == estudiante.IdEstudiante)
                .AsNoTracking();

            if (fecha.HasValue)
            {
                query = query.Where(r => r.FechaHora.Date == fecha.Value.Date);
            }

            if (!string.IsNullOrWhiteSpace(serial))
            {
                query = query.Where(r => r.Equipo != null && r.Equipo.Serial.Contains(serial));
            }

            var historial = await query
                .OrderByDescending(r => r.FechaHora)
                .Take(50)
                .Select(r => new RegistroAccesoReadDto
                {
                    IdRegistro = r.IdRegistro,
                    DocumentoEstudiante = estudiante.NumeroDocumento,
                    NombreEstudiante = $"{estudiante.Nombres} {estudiante.Apellidos}",
                    SerialEquipo = r.Equipo != null ? r.Equipo.Serial : "Sin Equipo",
                    NombreGuardia = r.Guardia != null ? r.Guardia.NombreCompleto : "Usuario Sistema",
                    IdPorteria = r.IdPorteria,
                    TipoMovimiento = r.TipoMovimiento,
                    FechaHora = r.FechaHora
                })
                .ToListAsync();

            return Ok(historial);
        }

        /// <summary>
        /// Reporte consolidado de movimientos para auditoría (Exclusivo SuperAdmin)
        /// </summary>
        [HttpGet("reporte")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ObtenerReporte(
            [FromQuery] DateTime? fechaInicio,
            [FromQuery] DateTime? fechaFin,
            [FromQuery] string? documento,
            [FromQuery] string? serial,
            [FromQuery] string? tipoMovimiento)
        {
            var query = _context.RegistrosAccesos
                .Include(r => r.Estudiante)
                .Include(r => r.Equipo)
                .Include(r => r.Guardia)
                .AsNoTracking();

            if (fechaInicio.HasValue)
                query = query.Where(r => r.FechaHora.Date >= fechaInicio.Value.Date);

            if (fechaFin.HasValue)
                query = query.Where(r => r.FechaHora.Date <= fechaFin.Value.Date);

            if (!string.IsNullOrWhiteSpace(documento))
                query = query.Where(r => r.Estudiante.NumeroDocumento.Contains(documento));

            if (!string.IsNullOrWhiteSpace(serial))
                query = query.Where(r => r.Equipo != null && r.Equipo.Serial.Contains(serial));

            if (!string.IsNullOrWhiteSpace(tipoMovimiento) && tipoMovimiento != "Todos")
                query = query.Where(r => r.TipoMovimiento == tipoMovimiento);

            var resultado = await query
                .OrderByDescending(r => r.FechaHora)
                .Select(r => new
                {
                    r.IdRegistro,
                    DocumentoEstudiante = r.Estudiante.NumeroDocumento,
                    NombreEstudiante = $"{r.Estudiante.Nombres} {r.Estudiante.Apellidos}",
                    SerialEquipo = r.Equipo != null ? r.Equipo.Serial : "Sin Equipo",
                    MarcaEquipo = r.Equipo != null && r.Equipo.Marca != null ? r.Equipo.Marca.NombreMarca : "N/A",
                    NombreGuardia = r.Guardia != null ? r.Guardia.NombreCompleto : "Sistema",
                    r.TipoMovimiento,
                    r.FechaHora
                })
                .ToListAsync();

            return Ok(resultado);
        }
    }
}