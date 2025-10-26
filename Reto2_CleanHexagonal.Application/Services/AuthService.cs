using Reto2_CleanHexagonal.Domain.Models;
using Reto2_CleanHexagonal.Domain.Ports;
using System.Security.Cryptography;
using System.Text;

namespace Reto2_CleanHexagonal.Application.Services
{
    /// <summary>
    /// Servicio de autenticación - Implementa la lógica de login y registro
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        }

        public async Task<(User user, string token)> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("El nombre de usuario es requerido", nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña es requerida", nameof(password));

            // Buscar usuario
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            // Verificar si el usuario está activo
            if (!user.IsActive)
                throw new UnauthorizedAccessException("La cuenta está desactivada");

            // Verificar contraseña
            if (!VerifyPassword(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            // Actualizar último login
            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);

            // Generar token JWT
            var token = _jwtTokenGenerator.GenerateToken(user);

            return (user, token);
        }

        public async Task<(User user, string token)> RegisterAsync(string username, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("El nombre de usuario es requerido", nameof(username));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email es requerido", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("La contraseña es requerida", nameof(password));

            if (password.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres", nameof(password));

            // Verificar si el usuario ya existe
            if (await _userRepository.ExistsAsync(username))
                throw new InvalidOperationException("El nombre de usuario ya está en uso");

            var existingEmail = await _userRepository.GetByEmailAsync(email);
            if (existingEmail != null)
                throw new InvalidOperationException("El email ya está registrado");

            // Hash de la contraseña
            var passwordHash = HashPassword(password);

            // Crear nuevo usuario
            var user = new User(username, email, passwordHash, "User");
            await _userRepository.CreateAsync(user);

            // Generar token JWT
            var token = _jwtTokenGenerator.GenerateToken(user);

            return (user, token);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == passwordHash;
        }
    }
}
