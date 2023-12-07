using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plyfood.Context;
using Plyfood.Dto.CartItems;
using Plyfood.Entity;
using Plyfood.Service.IService;

namespace Plyfood.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CartItemController : Controller
{
    private readonly ICartItemService _cartItemService;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;

    public CartItemController(ICartItemService cartItemService,ITokenService tokenService,AppDbContext context)
    {
        _cartItemService = cartItemService;
        _tokenService = tokenService;
        _context = context;
    }

    [HttpPost("addToCart")]
    [Authorize]
    public IActionResult AddToCart([FromBody] CreatingCartItemDto cartItemDto)
    {
        var cartItem = cartItemDto.ChangeToCart();
        var account = GetAccountFromHeader();
        cartItem.Cart_Id = account.Users.FirstOrDefault().Carts.FirstOrDefault().Cart_Id;
        return Ok(_cartItemService.Save(cartItem));
    }
    
    
    private Account GetAccountFromHeader()
    {
        string tokenRequestHeader =  HttpContext.Request.Headers["Authorization"];
        var token = _tokenService.ExtractTokenFromHeader(tokenRequestHeader);
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity.Name;
        var account =  _context.Accounts
            .Include(x=> x.Users)
            .ThenInclude(x=> x.Carts)
            .FirstOrDefault(x=> x.User_name == username);
        return account;
    }
}