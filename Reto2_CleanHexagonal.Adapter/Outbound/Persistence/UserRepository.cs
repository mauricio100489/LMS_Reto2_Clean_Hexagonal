using Reto2_CleanHexagonal.Domain.Models;
using Reto2_CleanHexagonal.Domain.Ports;

namespace Reto2_CleanHexagonal.Adapter.Outbound.Persistence
{
    /// <summary>
    /// Adaptador de salida para persistencia de usuarios (implementación en memoria)
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users = new();
        private readonly object _lock = new();

        public UserRepository()
        {
            // Crear usuario de prueba: admin/admin123
            var adminPasswordHash = HashPassword("admin123");
            var adminUser = new User("admin", "admin@lms.com", adminPasswordHash, "Admin");
            _users.Add(adminUser);

            // Crear usuario de prueba: user/user123
            var userPasswordHash = HashPassword("user123");
            var regularUser = new User("user", "user@lms.com", userPasswordHash, "User");
            _users.Add(regularUser);
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                return Task.FromResult(user);
            }
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                return Task.FromResult(user);
            }
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u =>
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                return Task.FromResult(user);
            }
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult<IEnumerable<User>>(_users.ToList());
            }
        }

        public Task<User> CreateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            lock (_lock)
            {
                _users.Add(user);
                return Task.FromResult(user);
            }
        }

        public Task<User> UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            lock (_lock)
            {
                var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser == null)
                    throw new KeyNotFoundException($"Usuario con ID {user.Id} no encontrado");

                var index = _users.IndexOf(existingUser);
                _users[index] = user;

                return Task.FromResult(user);
            }
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.Id == id);
                if (user == null)
                    return Task.FromResult(false);

                _users.Remove(user);
                return Task.FromResult(true);
            }
        }

        public Task<bool> ExistsAsync(string username)
        {
            lock (_lock)
            {
                var exists = _users.Any(u =>
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
                return Task.FromResult(exists);
            }
        }
    }
}
