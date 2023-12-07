using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plyfood.Context;
using Plyfood.Entity;
using Plyfood.Service.IService;

namespace Plyfood.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;

    public CartController(ICartService cartService, ITokenService tokenService,AppDbContext context)
    {
        _cartService = cartService;
        _tokenService = tokenService;
        _context = context;
    }

    [HttpGet("view")]
    [Authorize(Roles = "User")]
    public IActionResult ViewCart()
    {
        var account = GetAccountLogin();
        return Ok(_cartService.ViewCart(account.Users.FirstOrDefault().Carts.FirstOrDefault()));
    }

    [HttpPut("clear")]
    [Authorize(Roles = "User")]
    public IActionResult Clear()
    {
        return Ok(_cartService.ClearCart(GetAccountLogin().Users.FirstOrDefault().User_Id));
    }

    [HttpPost("cartToOrder")]
    [Authorize(Roles = "User")]
    public IActionResult CartToOrder()
    {
        return Ok(_cartService.CartToOrder(GetAccountLogin().Users.FirstOrDefault().User_Id));
    }

    
    private Account GetAccountLogin()
    {
        var headerToken = HttpContext.Request.Headers.Authorization;
        var token = _tokenService.ExtractTokenFromHeader(headerToken);
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity.Name;
        var account = 
            _context.Accounts
                .Include(x=> x.Users)
                .ThenInclude(x=> x.Carts)
                .ThenInclude(x=> x.Items)
                .FirstOrDefault(x=> x.User_name == username);
        return account;
    }
}