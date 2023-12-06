using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Plyfood.Entity;
using Plyfood.Helper;
using Plyfood.ResponseEntity;
using Plyfood.Service.IService;

namespace Plyfood.Service.Impl;

public class JwtService : IJwtService
{
    private readonly AppSettings _jwtConfig;

    public JwtService(AppSettings jwtConfig)
    {
        _jwtConfig = jwtConfig;
    }

    public string? CreateToken(Account account)
    {
        if (account == null)
        {
            return null;
        }

        if (_jwtConfig.Secrets == null)
        {
            Console.WriteLine("Lỗi Key");
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtConfig.Secrets);
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, account.Users.FirstOrDefault(x => x.User_Name.Equals(account.User_name)).Email),
            new Claim(ClaimTypes.Name, account.User_name),
            new Claim(ClaimTypes.Role, account.Decentralization.Authority_name)
        };

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Audience = _jwtConfig.ValidAudience,
            Issuer = _jwtConfig.ValidIssuer,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string? ValidateToken(string token)
    {
        throw new NotImplementedException();
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        if (_jwtConfig.Secrets != null)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secrets)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal =
                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        return null;
    }

    public bool IsTokenExpired(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jsonToken == null)
        {
            return true;
        }

        var expiration = jsonToken.ValidTo;

        return expiration != null && expiration < DateTime.UtcNow;
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}