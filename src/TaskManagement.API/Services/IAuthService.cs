using TaskManagement.API.DTOs;

namespace TaskManagement.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto request);
    Task<AuthResponseDto?> LoginAsync(LoginDto request);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
