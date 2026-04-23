using Lab2.DTOs.StudentDTOS;
using Lab3.DTOs;

namespace Lab3.Services.interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDTO dto);

        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);

        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken);

        Task RevokeTokenAsync(string userId, string refreshToken);
    }

}
