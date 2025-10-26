using Reto2_CleanHexagonal.Domain.Models;

namespace Reto2_CleanHexagonal.Domain.Ports
{
    /// <summary>
    /// Puerto de salida para gestión de usuarios
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(string username);
    }
}
