using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IAccountService _accountService;

    public CartItemController(ICartItemService cartItemService,ITokenService tokenService,IAccountService accountService)
    {
        _cartItemService = cartItemService;
        _tokenService = tokenService;
        _accountService = accountService;
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
        var headerToken = HttpContext.Request.Headers.Authorization;
        var token = _tokenService.ExtractTokenFromHeader(headerToken);
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity.Name;
        var account = _accountService.FindByUsername(username);
        return account;
    }
}