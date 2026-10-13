using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGE.Backend.Data;
using SGE.Backend.DTOs.Equipos;
using SGE.Backend.Models;

namespace SGE.Backend.Controllers.Equipos
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protección con JWT y Control de Acceso por Roles RBAC (RNF-08)
    public class EquiposController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EquiposController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// RF-04 / RNF-01: Consultar los equipos registrados a un estudiante por su documento.
        /// Tiempos de respuesta < 2.0s optimizado con AsNoTracking().
        /// </summary>
        [HttpGet("estudiante/{numeroDocumento}")]
        public async Task<IActionResult> ObtenerEquiposPorEstudiante(string numeroDocumento)
        {
            var estudiante = await _context.Estudiantes
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.NumeroDocumento == numeroDocumento);

            if (estudiante == null)
            {
                return NotFound(new { mensaje = "El estudiante no se encuentra registrado en el sistema." });
            }

            var equipos = await _context.Equipos
                .Include(e => e.Marca)     // Propiedad navegación exact: Marca
                .Include(e => e.Tipo)      // Propiedad navegación exact: Tipo
                .Include(e => e.Accesorios)// Propiedad navegación exact: Accesorios
                .Where(e => e.IdEstudiante == estudiante.IdEstudiante)
                .AsNoTracking() // Optimización para RNF-01 (< 2.0s)
                .Select(e => new EquipoReadDto
                {
                    IdEquipo = e.IdEquipo,
                    IdEstudiante = e.IdEstudiante,
                    Serial = e.Serial,
                    Marca = e.Marca != null ? e.Marca.NombreMarca : "N/A",
                    Tipo = e.Tipo != null ? e.Tipo.NombreTipo : "N/A",
                    FotoEquipoUrl = e.FotoEquipoUrl,
                    Estado = e.Estado,
                    Accesorios = e.Accesorios.Select(a => new AccesorioReadDto
                    {
                        IdAccesorio = a.IdAccesorio,
                        DescripcionAccesorio = a.DescripcionAccesorio ?? string.Empty,
                        FotoAccesoriosUrl = a.FotoAccesoriosUrl
                    }).ToList()
                })
                .ToListAsync();

            return Ok(equipos);
        }

        /// <summary>
        /// RF-05 / RF-06: Registrar un nuevo equipo portátil y sus accesorios a un estudiante.
        /// Exclusivo para perfiles Administrador y SuperAdmin.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Administrador")]
        public async Task<IActionResult> RegistrarEquipo([FromBody] CrearEquipoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Validar que el estudiante exista en la base de datos
            var estudianteExiste = await _context.Estudiantes.AnyAsync(e => e.IdEstudiante == dto.IdEstudiante);
            if (!estudianteExiste)
                return BadRequest(new { mensaje = "El ID del estudiante especificado no existe." });

            // 2. Validar regla de unicidad en serial (Restricción UNIQUE en SQL Server)
            var serialDuplicado = await _context.Equipos.AnyAsync(e => e.Serial == dto.Serial);
            if (serialDuplicado)
                return BadRequest(new { mensaje = "El número de serial ingresado ya pertenece a otro equipo registrado." });

            // 3. Crear el nuevo registro en la tabla 'equipos'
            var nuevoEquipo = new Equipo
            {
                IdEstudiante = dto.IdEstudiante,
                IdTipo = dto.IdTipo,
                IdMarca = dto.IdMarca,
                Serial = dto.Serial,
                FotoEquipoUrl = dto.FotoEquipoUrl,
                Estado = "Activo"
            };

            await _context.Equipos.AddAsync(nuevoEquipo);
            await _context.SaveChangesAsync(); // Se asigna IdEquipo en SQL Server

            // 4. Si se incluyen accesorios, registrarlos en la tabla 'accesorios_equipo' (RF-06)
            if (!string.IsNullOrEmpty(dto.FotoAccesoriosUrl) || !string.IsNullOrEmpty(dto.DescripcionAccesorio))
            {
                var accesorio = new AccesorioEquipo
                {
                    IdEquipo = nuevoEquipo.IdEquipo,
                    DescripcionAccesorio = dto.DescripcionAccesorio ?? "Cargador y periféricos estándar",
                    FotoAccesoriosUrl = dto.FotoAccesoriosUrl
                };

                await _context.AccesoriosEquipos.AddAsync(accesorio);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(ObtenerEquiposPorEstudiante), new { numeroDocumento = dto.IdEstudiante }, new
            {
                mensaje = "Equipo y accesorios registrados exitosamente.",
                nuevoEquipo.IdEquipo,
                nuevoEquipo.Serial
            });
        }

        /// <summary>
        /// RF-09: Cambiar el estado operativo del equipo (Activo / Inactivo / Reportado).
        /// Exclusivo para Administradores y SuperAdmin.
        /// </summary>
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "SuperAdmin,Administrador")]
        public async Task<IActionResult> CambiarEstadoEquipo(int id, [FromBody] string nuevoEstado)
        {
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null)
                return NotFound(new { mensaje = "Equipo no encontrado." });

            equipo.Estado = nuevoEstado;
            _context.Equipos.Update(equipo);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Estado del equipo actualizado a '{nuevoEstado}' de forma exitosa." });
        }
    }
}