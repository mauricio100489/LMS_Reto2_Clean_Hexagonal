using Reto2_CleanHexagonal.Domain.Models;

namespace Reto2_CleanHexagonal.Domain.Ports
{
    /// <summary>
    /// Puerto de salida para generación de tokens JWT
    /// </summary>
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
        Guid? ValidateToken(string token);
    }
}
