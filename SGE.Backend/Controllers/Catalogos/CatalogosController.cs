using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGE.Backend.Data;

namespace SGE.Backend.Controllers.Catalogos
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protegido por autenticación/sesión
    public class CatalogosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CatalogosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Obtener todas las Facultades
        [HttpGet("facultades")]
        public async Task<IActionResult> GetFacultades()
        {
            var facultades = await _context.Facultades
                .AsNoTracking()
                .Select(f => new
                {
                    idFacultad = f.IdFacultad,
                    nombreFacultad = f.NombreFacultad
                })
                .ToListAsync();

            return Ok(facultades);
        }

        // 2. Obtener Carreras filtradas por ID de Facultad
        [HttpGet("carreras/facultad/{idFacultad}")]
        public async Task<IActionResult> GetCarrerasPorFacultad(int idFacultad)
        {
            var carreras = await _context.Carreras
                .Where(c => c.IdFacultad == idFacultad)
                .AsNoTracking()
                .Select(c => new
                {
                    idCarrera = c.IdCarrera,
                    nombreCarrera = c.NombreCarrera
                })
                .ToListAsync();

            return Ok(carreras);
        }

        // 3. Obtener Marcas de Equipos
        [HttpGet("marcas")]
        public async Task<IActionResult> GetMarcas()
        {
            var marcas = await _context.MarcasEquipos
                .AsNoTracking()
                .Select(m => new
                {
                    idMarca = m.IdMarca,
                    nombreMarca = m.NombreMarca
                })
                .ToListAsync();

            return Ok(marcas);
        }

        // 4. Obtener Tipos de Equipo
        [HttpGet("tipos-equipo")]
        public async Task<IActionResult> GetTiposEquipo()
        {
            var tipos = await _context.TiposEquipos
                .AsNoTracking()
                .Select(t => new
                {
                    idTipo = t.IdTipo,
                    nombreTipo = t.NombreTipo
                })
                .ToListAsync();

            return Ok(tipos);
        }

        // 5. Obtener todas las Porterías
        [HttpGet("porterias")]
        public async Task<IActionResult> GetPorterias()
        {
            var porterias = await _context.Porterias
                .AsNoTracking()
                .Select(p => new
                {
                    idPorteria = p.IdPorteria,
                    nombrePorteria = p.NombrePorteria
                })
                .ToListAsync();

            return Ok(porterias);
        }
    }
}