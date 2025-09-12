using GestionApi.Models;

namespace GestionApi.Services
{
    public interface IJwtService
    {
        string GenerateToken(Usuario usuario);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        string? GetUserIdFromToken(string token);
    }
}
