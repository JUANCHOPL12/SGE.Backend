using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGE.Backend.Data;
using SGE.Backend.DTOs;
using SGE.Backend.Models;
using BCrypt.Net;

namespace SGE.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")] // Restricción RBAC: solo ejecutable por el perfil SuperAdmin (RNF-08)
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios (Obtener la lista completa de cuentas)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioReadDto>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .Select(u => new UsuarioReadDto
                {
                    IdUsuario = u.IdUsuario,
                    NombreUsuario = u.NombreUsuario,
                    NombreCompleto = u.NombreCompleto,
                    Estado = u.Estado,
                    IdRol = u.IdRol,
                    NombreRol = u.Rol != null ? u.Rol.NombreRol : "Sin Rol"
                })
                .ToListAsync();

            return Ok(usuarios);
        }

        // GET: api/Usuarios/5 (Consultar una cuenta por su ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioReadDto>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            var dto = new UsuarioReadDto
            {
                IdUsuario = usuario.IdUsuario,
                NombreUsuario = usuario.NombreUsuario,
                NombreCompleto = usuario.NombreCompleto,
                Estado = usuario.Estado,
                IdRol = usuario.IdRol,
                NombreRol = usuario.Rol != null ? usuario.Rol.NombreRol : "Sin Rol"
            };

            return Ok(dto);
        }

        // POST: api/Usuarios (Crear cuentas de Administrador o Guardia - RF-10)
        [HttpPost]
        public async Task<ActionResult<UsuarioReadDto>> CreateUsuario([FromBody] UsuarioCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool existeUsuario = await _context.Usuarios.AnyAsync(u => u.NombreUsuario.ToLower() == dto.NombreUsuario.ToLower());
            if (existeUsuario)
            {
                return BadRequest(new { mensaje = "El nombre de usuario ya está registrado." });
            }

            var rol = await _context.Roles.FindAsync(dto.IdRol);
            if (rol == null)
            {
                return BadRequest(new { mensaje = "El Rol especificado no existe." });
            }

            // Cifrado de contraseña con BCrypt (RNF-03)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var nuevoUsuario = new Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                PasswordHash = passwordHash,
                NombreCompleto = dto.NombreCompleto,
                Estado = "Activo",
                IdRol = dto.IdRol
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            var responseDto = new UsuarioReadDto
            {
                IdUsuario = nuevoUsuario.IdUsuario,
                NombreUsuario = nuevoUsuario.NombreUsuario,
                NombreCompleto = nuevoUsuario.NombreCompleto,
                Estado = nuevoUsuario.Estado,
                IdRol = nuevoUsuario.IdRol,
                NombreRol = rol.NombreRol
            };

            return CreatedAtAction(nameof(GetUsuario), new { id = nuevoUsuario.IdUsuario }, responseDto);
        }

        // PUT: api/Usuarios/5 (Editar información y cambiar estado - RF-10)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioUpdateDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            var rol = await _context.Roles.FindAsync(dto.IdRol);
            if (rol == null)
            {
                return BadRequest(new { mensaje = "El Rol especificado no existe." });
            }

            usuario.NombreCompleto = dto.NombreCompleto;
            usuario.Estado = dto.Estado;
            usuario.IdRol = dto.IdRol;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario actualizado correctamente." });
        }

        // PATCH: api/Usuarios/5/desactivar (Desactivación de cuentas - RF-10)
        [HttpPatch("{id}/desactivar")]
        public async Task<IActionResult> DesactivarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado." });
            }

            usuario.Estado = "Inactivo";
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario desactivado exitosamente." });
        }
    }
}