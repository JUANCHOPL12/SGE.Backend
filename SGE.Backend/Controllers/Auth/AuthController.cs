using Microsoft.AspNetCore.Mvc;
using SGE.Backend.DTOs.Auth;
using SGE.Backend.Services;

namespace SGE.Backend.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Endpoint para el inicio de sesión de usuarios (RF-01, RF-03)
        /// POST: api/auth/login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // 1. Validar que la solicitud cumpla con los requisitos del DTO ([Required])
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 2. Procesar la autenticación a través del servicio
            var response = await _authService.AuthenticateAsync(loginDto);

            // 3. Si las credenciales no son válidas o el usuario está inactivo (RF-03)
            if (response == null)
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas o usuario inactivo." });
            }

            // 4. Retornar respuesta 200 OK con el Token JWT y perfil del usuario (RF-01)
            return Ok(response);
        }
    }
}