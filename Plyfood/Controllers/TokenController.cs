using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plyfood.Context;
using Plyfood.Dto.Token;
using Plyfood.Entity;
using Plyfood.ResponseEntity;
using Plyfood.Service.IService;

namespace Plyfood.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TokenController : Controller
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public TokenController(AppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost]
    [Route("refreshToken")]
    public IActionResult RefreshToken(TokenApiModel apiModel)
    {
        if (apiModel is null)
        {
            return BadRequest("Invalid client request");
        }

        string accessToken = apiModel.AccessToken;
        string refreshToken = apiModel.RefreshToken;

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        var userName = principal.Identity.Name;

        var account = _context.Accounts.FirstOrDefault(x => x.User_name.Equals(userName));
        var tokenAccount = _context.Tokens.FirstOrDefault(x => x.AccountId == account.Account_Id);
        if (account is null || tokenAccount.RefreshToken != refreshToken || tokenAccount.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest("Invalid client request");
        }

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList());

        account.Token.RefreshToken = newRefreshToken;
        _context.Accounts.Update(account);
        _context.SaveChanges();
        return Ok(new AuthenticationResponse()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
    
    [HttpPost, Authorize]
    [Route("revoke")]
    public IActionResult Revoke()
    {
        var username = User.Identity.Name;
        var user = _context.Accounts.SingleOrDefault(u => u.User_name == username);
        if (user == null) return BadRequest();
        user.Token= null;
        _context.SaveChanges();
        return NoContent();
    }
}