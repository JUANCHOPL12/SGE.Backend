using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGE.Backend.Data;
using SGE.Backend.DTOs;
using SGE.Backend.Models;

namespace SGE.Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> AuthenticateAsync(LoginDto loginDto)
        {
            // 1. Consulta al usuario en la BD incluyendo su Rol
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.NombreUsuario == loginDto.NombreUsuario);

            // 2. Verifica si el usuario existe y si su estado es "Activo"
            if (usuario == null || usuario.Estado != "Activo")
            {
                return null;
            }

            // 3. Valida la contraseña comparando el texto plano con el Hash BCrypt (RNF-03)
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, usuario.PasswordHash);
            if (!isPasswordValid)
            {
                return null;
            }

            // 4. Genera el Token JWT con los Claims necesarios para RBAC (RNF-08)
            string token = GenerarJwtToken(usuario);

            // 5. Retorna la respuesta con el Token y la información del perfil
            return new LoginResponseDto
            {
                Token = token,
                IdUsuario = usuario.IdUsuario,
                NombreCompleto = usuario.NombreCompleto,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol?.NombreRol ?? "Sin Rol"
            };
        }

        private string GenerarJwtToken(Usuario usuario)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("Clave JWT no configurada en appsettings.json"));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim("NombreCompleto", usuario.NombreCompleto),
                new Claim(ClaimTypes.Role, usuario.Rol?.NombreRol ?? "Sin Rol")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}