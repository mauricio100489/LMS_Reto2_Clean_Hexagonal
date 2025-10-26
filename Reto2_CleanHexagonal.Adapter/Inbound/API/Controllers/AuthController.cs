using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Reto2_CleanHexagonal.Adapter.Inbound.API.DTOs;
using Reto2_CleanHexagonal.Domain.Ports;
using System.Security.Claims;

namespace Reto2_CleanHexagonal.Adapter.Inbound.API.Controllers
{
    /// <summary>
    /// Controlador de autenticación - Maneja login, registro y información de usuario
    /// </summary>
    
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Login de usuario - Retorna JWT token
        /// </summary>
        /// <remarks>
        /// Usuarios de prueba:
        /// - admin/admin123 (Rol: Admin)
        /// - user/user123 (Rol: User)
        /// </remarks>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            _logger.LogInformation("Intento de login para usuario: {Username}", request.Username);

            try
            {
                var (user, token) = await _authService.LoginAsync(request.Username, request.Password);

                var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
                var response = new AuthResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
                };

                _logger.LogInformation("Login exitoso para usuario: {Username}", user.Username);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Login fallido para usuario: {Username} - {Message}", request.Username, ex.Message);
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Datos de login inválidos: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Registro de nuevo usuario
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            _logger.LogInformation("Intento de registro para usuario: {Username}", request.Username);

            try
            {
                var (user, token) = await _authService.RegisterAsync(
                    request.Username,
                    request.Email,
                    request.Password);

                var expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
                var response = new AuthResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
                };

                _logger.LogInformation("Registro exitoso para usuario: {Username}", user.Username);
                return CreatedAtAction(nameof(GetMe), null, response);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Registro fallido: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Datos de registro inválidos: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene información del usuario autenticado
        /// Requiere token JWT válido
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserInfoDto>> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Token JWT inválido o sin información de usuario");
                return Unauthorized();
            }

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Usuario con ID {UserId} no encontrado", userId);
                return NotFound(new { message = "Usuario no encontrado" });
            }

            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return Ok(userInfo);
        }

        /// <summary>
        /// Endpoint de prueba para verificar el token JWT
        /// </summary>
        [HttpGet("verify")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult VerifyToken()
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value
                ?? User.FindFirst("unique_name")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                message = "Token válido",
                username = username,
                role = role,
                authenticated = true
            });
        }
    }
}
