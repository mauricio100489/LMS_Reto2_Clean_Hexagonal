using Reto2_CleanHexagonal.Domain.Models;

namespace Reto2_CleanHexagonal.Domain.Ports
{
    /// <summary>
    /// Puerto de entrada para autenticación
    /// </summary>
    public interface IAuthService
    {
        Task<(User user, string token)> LoginAsync(string username, string password);
        Task<(User user, string token)> RegisterAsync(string username, string email, string password);
        Task<User?> GetUserByIdAsync(Guid userId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}
