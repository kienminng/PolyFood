using System.Security.Claims;
using Plyfood.Entity;
using Plyfood.ResponseEntity;

namespace Plyfood.Service.IService;

public interface IJwtService
{
    string? CreateToken(Account account);
    string? ValidateToken(string token);

    bool IsTokenExpired(string token);
    
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

    string GenerateRefreshToken();

}