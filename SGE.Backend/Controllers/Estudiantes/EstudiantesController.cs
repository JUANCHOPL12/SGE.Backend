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
                    FotoAccesoriosUrl = e.Equipos.Select(eq => eq.FotoAccesoriosUrl).FirstOrDefault(),
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
                return NotFound(new { mensaje = "El número de documento no corresponde a un registro existente." });
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
                FotoAccesoriosUrl = equipo?.FotoAccesoriosUrl,
                SerialEquipo = equipo?.Serial,
                TieneEquipoRegistrado = equipo != null
            };

            return Ok(dto);
        }

        // 3. Crear Estudiante (RF-05) y vincular Equipo Opcional (RF-06)
        [HttpPost]
        [Authorize(Roles = "Administrador,SuperAdmin")]
        public async Task<ActionResult> CreateEstudiante([FromBody] EstudianteCreateUpdateDto dto)
        {
            if (await _context.Estudiantes.AnyAsync(e => e.NumeroDocumento == dto.Documento))
            {
                return BadRequest(new { mensaje = "Ya existe un estudiante registrado con este documento." });
            }

            // Procesar y guardar foto de perfil
            var fotoPerfilRuta = GuardarImagenBase64(dto.FotoPerfilUrl, "estudiantes");

            var estudiante = new Estudiante
            {
                NumeroDocumento = dto.Documento,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Telefono = dto.Telefono,
                CorreoElectronico = dto.Correo,
                IdCarrera = dto.IdCarrera,
                Estado = "Activo",
                FotoPerfilUrl = fotoPerfilRuta
            };

            _context.Estudiantes.Add(estudiante);
            await _context.SaveChangesAsync();

            // Guardar equipo SOLO SI el serial no está vacío Y se enviaron IDs de marca y tipo mayores a 0
            if (!string.IsNullOrWhiteSpace(dto.SerialEquipo)
                && dto.IdMarca.HasValue && dto.IdMarca.Value > 0
                && dto.IdTipo.HasValue && dto.IdTipo.Value > 0)
            {
                var fotoEquipoRuta = GuardarImagenBase64(dto.FotoEquipoUrl, "equipos");
                var fotoAccesoriosRuta = GuardarImagenBase64(dto.FotoAccesoriosUrl, "accesorios");

                var equipo = new Equipo
                {
                    IdEstudiante = estudiante.IdEstudiante,
                    Serial = dto.SerialEquipo,
                    IdMarca = dto.IdMarca.Value,
                    IdTipo = dto.IdTipo.Value,
                    FotoEquipoUrl = fotoEquipoRuta ?? string.Empty,
                    FotoAccesoriosUrl = fotoAccesoriosRuta ?? string.Empty,
                    Estado = "Activo"
                };

                _context.Equipos.Add(equipo);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetEstudiantePorDocumento), new { documento = estudiante.NumeroDocumento }, dto);
        }

        // 4. Actualizar datos de Estudiante y su Equipo Opcional (RF-05, RF-06)
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

            // 1. Actualizar datos del estudiante
            estudiante.NumeroDocumento = dto.Documento;
            estudiante.Nombres = dto.Nombres;
            estudiante.Apellidos = dto.Apellidos;
            estudiante.Telefono = dto.Telefono;
            estudiante.CorreoElectronico = dto.Correo;
            estudiante.IdCarrera = dto.IdCarrera;

            // Procesar foto de perfil solo si se envió un valor nuevo
            if (!string.IsNullOrWhiteSpace(dto.FotoPerfilUrl))
            {
                estudiante.FotoPerfilUrl = GuardarImagenBase64(dto.FotoPerfilUrl, "estudiantes") ?? estudiante.FotoPerfilUrl;
            }

            // 2. Actualizar o Vincular Equipo
            if (!string.IsNullOrWhiteSpace(dto.SerialEquipo)
                && dto.IdMarca.HasValue && dto.IdMarca.Value > 0
                && dto.IdTipo.HasValue && dto.IdTipo.Value > 0)
            {
                var equipoExistente = estudiante.Equipos.FirstOrDefault();
                var fotoEquipoRuta = GuardarImagenBase64(dto.FotoEquipoUrl, "equipos");
                var fotoAccesoriosRuta = GuardarImagenBase64(dto.FotoAccesoriosUrl, "accesorios");

                if (equipoExistente != null)
                {
                    // ACTUALIZAR REGISTRO EXISTENTE
                    equipoExistente.Serial = dto.SerialEquipo;
                    equipoExistente.IdMarca = dto.IdMarca.Value;
                    equipoExistente.IdTipo = dto.IdTipo.Value;
                    if (!string.IsNullOrWhiteSpace(fotoEquipoRuta))
                    {
                        equipoExistente.FotoEquipoUrl = fotoEquipoRuta;
                    }
                    if (!string.IsNullOrWhiteSpace(fotoAccesoriosRuta))
                    {
                        equipoExistente.FotoAccesoriosUrl = fotoAccesoriosRuta;
                    }
                }
                else
                {
                    // INSERTAR SOLO SI NO TIENE EQUIPO PREVIO
                    var nuevoEquipo = new Equipo
                    {
                        IdEstudiante = estudiante.IdEstudiante,
                        Serial = dto.SerialEquipo,
                        IdMarca = dto.IdMarca.Value,
                        IdTipo = dto.IdTipo.Value,
                        FotoEquipoUrl = fotoEquipoRuta ?? string.Empty,
                        FotoAccesoriosUrl = fotoAccesoriosRuta ?? string.Empty,
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

        // =========================================================================
        // HELPER PRIVATE METHOD: Convierte imágenes Base64 a archivos en disco
        // =========================================================================
        private string? GuardarImagenBase64(string? base64String, string carpeta)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return null;

            // Si ya viene como URL fija o relativa, se deja intacta
            if (!base64String.StartsWith("data:image"))
                return base64String;

            try
            {
                var base64Data = base64String.Split(',')[1];
                var bytes = Convert.FromBase64String(base64Data);

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", carpeta);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = $"{Guid.NewGuid()}.jpg";
                var filePath = Path.Combine(folderPath, fileName);

                System.IO.File.WriteAllBytes(filePath, bytes);

                // Construye la URL completa incluyendo protocolo, puerto y servidor (ej: https://localhost:7243/uploads/estudiantes/guid.jpg)
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                return $"{baseUrl}/uploads/{carpeta}/{fileName}";
            }
            catch
            {
                return null;
            }
        }
    }
}