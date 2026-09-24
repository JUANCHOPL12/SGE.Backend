using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGE.Backend.Data;
using SGE.Backend.DTOs;
using SGE.Backend.DTOs.EstudiantesDtos;
using SGE.Backend.Models;

namespace SGE.Backend.Controllers.Estudiantes
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // RNF-08: Restricción por sesiones y roles (RBAC)
    public class EstudiantesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EstudiantesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Obtener todos los estudiantes (Administrador y SuperAdmin) - RF-05
        [HttpGet]
        [Authorize(Roles = "Administrador,SuperAdmin")]
        public async Task<ActionResult<IEnumerable<EstudianteReadDto>>> GetEstudiantes()
        {
            var estudiantes = await _context.Estudiantes
                .Include(e => e.Equipos)
                .AsNoTracking()
                .Select(e => new EstudianteReadDto
                {
                    IdEstudiante = e.IdEstudiante,
                    Documento = e.NumeroDocumento,
                    Nombres = e.Nombres,
                    Apellidos = e.Apellidos,
                    Telefono = e.Telefono,
                    Correo = e.CorreoElectronico,
                    IdCarrera = e.IdCarrera,
                    Estado = e.Estado,
                    FotoPerfilUrl = e.FotoPerfilUrl,
                    FotoEquipoUrl = e.Equipos.Select(eq => eq.FotoEquipoUrl).FirstOrDefault(),
                    SerialEquipo = e.Equipos.Select(eq => eq.Serial).FirstOrDefault(),
                    IdMarca = e.Equipos.Select(eq => (int?)eq.IdMarca).FirstOrDefault(),
                    IdTipo = e.Equipos.Select(eq => (int?)eq.IdTipo).FirstOrDefault()
                })
                .ToListAsync();

            return Ok(estudiantes);
        }

        // 2. Consulta de Estudiante por Documento - Autoservicio y Portería (RF-04, RF-07, RNF-01)
        [HttpGet("buscar/{documento}")]
        [Authorize(Roles = "Guardia,Administrador,SuperAdmin")]
        public async Task<ActionResult<EstudianteDetalleDto>> GetEstudiantePorDocumento(string documento)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.Equipos)
                .AsNoTracking() // RNF-01: Garantiza respuesta < 2.0s al omitir rastreo en memoria
                .FirstOrDefaultAsync(e => e.NumeroDocumento == documento);

            if (estudiante == null)
            {
                return NotFound(new { mensaje = "El número de documento no corresponde a un registro existente." }); // RNF-06
            }

            var equipo = estudiante.Equipos.FirstOrDefault();

            var dto = new EstudianteDetalleDto
            {
                IdEstudiante = estudiante.IdEstudiante,
                Documento = estudiante.NumeroDocumento,
                NombreCompleto = $"{estudiante.Nombres} {estudiante.Apellidos}",
                IdCarrera = estudiante.IdCarrera,
                Estado = estudiante.Estado,
                FotoPerfilUrl = estudiante.FotoPerfilUrl,
                FotoEquipoUrl = equipo?.FotoEquipoUrl,
                SerialEquipo = equipo?.Serial,
                TieneEquipoRegistrado = equipo != null
            };

            return Ok(dto);
        }

        // 3. Crear Estudiante y vincular Equipo (RF-05, RF-06)
        [HttpPost]
        [Authorize(Roles = "Administrador,SuperAdmin")]
        public async Task<ActionResult> CreateEstudiante([FromBody] EstudianteCreateUpdateDto dto)
        {
            if (await _context.Estudiantes.AnyAsync(e => e.NumeroDocumento == dto.Documento))
            {
                return BadRequest(new { mensaje = "Ya existe un estudiante registrado con este documento." });
            }

            var estudiante = new Estudiante
            {
                NumeroDocumento = dto.Documento,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Telefono = dto.Telefono,
                CorreoElectronico = dto.Correo,
                IdCarrera = dto.IdCarrera,
                Estado = "Activo",
                FotoPerfilUrl = dto.FotoPerfilUrl
            };

            _context.Estudiantes.Add(estudiante);
            await _context.SaveChangesAsync();

            // Si se envió un serial, vinculamos el equipo en la tabla 'equipos'
            if (!string.IsNullOrEmpty(dto.SerialEquipo))
            {
                var equipo = new Equipo
                {
                    IdEstudiante = estudiante.IdEstudiante,
                    Serial = dto.SerialEquipo,
                    IdMarca = dto.IdMarca,
                    IdTipo = dto.IdTipo,
                    FotoEquipoUrl = dto.FotoEquipoUrl,
                    Estado = "Activo"
                };

                _context.Equipos.Add(equipo);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetEstudiantePorDocumento), new { documento = estudiante.NumeroDocumento }, dto);
        }

        // 4. Actualizar datos de Estudiante y su Equipo (RF-05, RF-06)
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador,SuperAdmin")]
        public async Task<IActionResult> UpdateEstudiante(int id, [FromBody] EstudianteCreateUpdateDto dto)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.Equipos)
                .FirstOrDefaultAsync(e => e.IdEstudiante == id);

            if (estudiante == null)
            {
                return NotFound(new { mensaje = "Estudiante no encontrado." });
            }

            estudiante.NumeroDocumento = dto.Documento;
            estudiante.Nombres = dto.Nombres;
            estudiante.Apellidos = dto.Apellidos;
            estudiante.Telefono = dto.Telefono;
            estudiante.CorreoElectronico = dto.Correo;
            estudiante.IdCarrera = dto.IdCarrera;
            estudiante.FotoPerfilUrl = dto.FotoPerfilUrl ?? estudiante.FotoPerfilUrl;

            if (!string.IsNullOrEmpty(dto.SerialEquipo))
            {
                var equipo = estudiante.Equipos.FirstOrDefault();
                if (equipo != null)
                {
                    equipo.Serial = dto.SerialEquipo;
                    equipo.IdMarca = dto.IdMarca;
                    equipo.IdTipo = dto.IdTipo;
                    equipo.FotoEquipoUrl = dto.FotoEquipoUrl ?? equipo.FotoEquipoUrl;
                }
                else
                {
                    var nuevoEquipo = new Equipo
                    {
                        IdEstudiante = estudiante.IdEstudiante,
                        Serial = dto.SerialEquipo,
                        IdMarca = dto.IdMarca,
                        IdTipo = dto.IdTipo,
                        FotoEquipoUrl = dto.FotoEquipoUrl,
                        Estado = "Activo"
                    };
                    _context.Equipos.Add(nuevoEquipo);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 5. Cambiar Estado del Estudiante Activo/Inactivo (RF-09)
        [HttpPatch("{id}/estado")]
        [Authorize(Roles = "Administrador,SuperAdmin")]
        public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
        {
            if (nuevoEstado != "Activo" && nuevoEstado != "Inactivo")
            {
                return BadRequest(new { mensaje = "El estado debe ser 'Activo' o 'Inactivo'." });
            }

            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante == null)
            {
                return NotFound(new { mensaje = "Estudiante no encontrado." });
            }

            estudiante.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"Estado del estudiante actualizado a {nuevoEstado}." });
        }
    }
}