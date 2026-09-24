using SGE.Backend.DTOs;

namespace SGE.Backend.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> AuthenticateAsync(LoginDto loginDto);
    }
}