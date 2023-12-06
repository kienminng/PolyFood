using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plyfood.Dto.Account;
using Plyfood.Service.IService;

namespace Plyfood.Controllers;
[ApiController]
[Route("api/v1/[controller]")]

public class AuthenticationController : Controller
{
    private readonly IAccountService _accountService;
    private readonly ITokenService _tokenService;
    private readonly IMailSender _mailSender;

    public AuthenticationController(IAccountService accountService,ITokenService tokenService,IMailSender mailSender)
    {
        _accountService = accountService;
        _tokenService = tokenService;
        _mailSender = mailSender;
    }

    [HttpPost("Register")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] RegisterForm registerForm)
    {
        var check = _accountService.Register(registerForm,"User");
        if (check)
        {
            var account = _accountService.FindByUsername(registerForm.UserName);
            if (account is null)
            {
                return BadRequest("register false");
            }
            var token =_accountService.CreateAccessToken(account);
            if (token!=null)
            {
                var mail = _mailSender.ConfirmEmailByUrl(account, token);
                return Ok("Register Success");
            }
            return BadRequest("Register false");
        }

        return BadRequest("Register false");
    }

    [HttpPost("createAdmin")]
    [Authorize(Roles = "Admin")]
    public IActionResult RegisterAdmin([FromBody] RegisterForm form)
    {
        var check = _accountService.Register(form,"Admin");
        if (check)
        {
            var account = _accountService.FindByUsername(form.UserName);
            if (account is null)
            {
                return BadRequest("register false");
            }
            var token =_accountService.CreateAccessToken(account);
            if (token!=null)
            {
                var mail = _mailSender.ConfirmEmailByUrl(account, token);
                return Ok("Register Success");
            }
            return BadRequest("Register false");
        }

        return BadRequest("Register false");
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    public IActionResult Login(AuthenRequest authenRequest)
    {
        var token = _accountService.Login(authenRequest);
        if (token != null)
        {
            return Ok(token);
        }
        return Unauthorized("Sai tài khoản hoặc mật khẩu");
    }

    [HttpGet("changeStatus")]
    [AllowAnonymous]
    public IActionResult ChangeStatus([FromQuery] string token)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        if (principal is null)
        {
            return BadRequest("Invalid client");
        }
        var username = principal.Identity.Name;
        if (username is null)
        {
            return BadRequest("Invalid username");
        }
        var check = _accountService.ChangeStatus(username);
        if (check)
        {
            return Ok("Your account is already to use");
        }

        return BadRequest("Can't confirm");
    }
}