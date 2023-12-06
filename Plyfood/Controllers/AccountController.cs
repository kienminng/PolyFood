using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Plyfood.Dto.Account;
using Plyfood.Service.IService;

namespace Plyfood.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : Controller
{
    private readonly ITokenService _tokenService;
    private readonly IAccountService _accountService;

    public AccountController(ITokenService tokenService, IAccountService accountService)
    {
        _tokenService = tokenService;
        _accountService = accountService;
    }

    [HttpPost("changePassword")]
    [Authorize]
    public IActionResult ChangePassword(PasswordModelApi api)
    {
        var tokenHeader = HttpContext.Request.Headers["Authorization"];
        if (tokenHeader.IsNullOrEmpty())
        {
            return BadRequest("Invalid or missing token");
        }
        var token = _tokenService.ExtractTokenFromHeader(tokenHeader);
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("Invalid or missing token");
        }
        if (api.NewPassword is null || api.OldPassword is null)
            return BadRequest("wrong password");
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity.Name;
        if (username is null)
        {
            return BadRequest("User not found");
        }
        var check = _accountService.ChangePassword(username, api.OldPassword, api.NewPassword);
        if (check)
        {
            return Ok("change password success");
        }

        return BadRequest("change password false");
    }

    [HttpPost("GenericResetPasswordToken")]
    [AllowAnonymous]
    public IActionResult GenericResetPasswordToken(string username)
    {
        var check = _accountService.GenericResetPasswordToken(username);
        if (check)
        {
            return Ok("Token was send to your email");
        }

        return BadRequest("Your username or email is not exist");
    }

    [HttpPost("resetPassword")]
    [AllowAnonymous]
    public IActionResult ResetPassword([FromBody] ResetPasswordApi api)
    {
        if (api.Username is null || api.NewPassword is null || api.RestPasswordToken is null)
        {
            return BadRequest("some data is null");
        }
        var check = _accountService.ResetPassword(api.Username, api.RestPasswordToken, api.NewPassword);
        if ( check)
        {
            return Ok("Change password success");
        }
        return BadRequest("change password false");
    }

    // public IActionResult DoiMatKhau(string matKhauCu, string matKhauMoi)
    // {
    //     int id = int.Parse(HttpContext.User.FindFirst("Id").Value);
    //     return Ok(_accountService.ChangePassword(id, matKhauCu, matKhauMoi))
    //     
    // }

    [HttpPost("update")]
    [Authorize]
    public IActionResult UpdateUser(UpdateAccountApi api)
    {
        var tokenHeader = HttpContext.Request.Headers["Authorization"];
        
        if (tokenHeader.IsNullOrEmpty())
        {
            return BadRequest("Invalid or missing token");
        }
        var token = _tokenService.ExtractTokenFromHeader(tokenHeader);
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized("Invalid or missing token");
        }

        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity.Name;
        var check = _accountService.UpdateAccount(api,username);
        if (check)
        {
            return Ok("update success");
        }
        return BadRequest("Update False");
    }


    [HttpPost("BandAccount")]
    [Authorize(Roles = "Admin")]
    public IActionResult BandAccount(int id)
    {
        var requestHeader = HttpContext.Request.Headers["Authorization"];
        var token = _tokenService.ExtractTokenFromHeader(requestHeader);
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        var role = principal.IsInRole("Admin");
        if (role)
        {
            return BadRequest("This user cannot be band");
        }
        var check = _accountService.BandAccount(id);
        if (check)
        {
            return Ok("Band success");
        }

        return BadRequest("Band False");
    }
}