using System.Security.Claims;

namespace Plyfood.Service.IService;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

    bool IsTokenExpired(string token);

    string? ValidateToken(string token);

    string ExtractTokenFromHeader(string authorizationHeader);
}