using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plyfood.Dto.Order;
using Plyfood.Entity;
using Plyfood.Migrations;
using Plyfood.Service.IService;

namespace Plyfood.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IAccountService _accountService;
    private readonly ITokenService _tokenService;

    public OrderController(IOrderService orderService,IAccountService  accountService,ITokenService tokenService)
    {
        _orderService = orderService;
        _accountService = accountService;
        _tokenService = tokenService;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public IActionResult Create([FromBody] OrderCreatingForm creatingForm)
    {
        string tokenRequestHeader =  HttpContext.Request.Headers["Authorization"];
        var token = _tokenService.ExtractTokenFromHeader(tokenRequestHeader);
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity.Name;
        var account = _accountService.FindByUsername(username);
        if (account is null )
        {
            return BadRequest("Lỗi token user không tồn tại trên hệ thống");
        }
        return Ok(_orderService.Create(creatingForm, account.Users.FirstOrDefault().User_Id));
    }

    [HttpPut("changeStatus")]
    [Authorize(Roles = "Admin")]
    public IActionResult ChangeStatus([FromQuery] int orderId,[FromBody] int orderStatusId)
    {
        return Ok(_orderService.ChangeStatus(orderId, orderStatusId));
    }

    [HttpGet("GetAllByUser")]
    [Authorize]
    public IActionResult GetAllByUser()
    {
        var account = GetAccountLogin();
        return Ok(_orderService.GetAllLoginUser(account.User_name));
    }

    [HttpGet("GetTotalPrice")]
    public IActionResult CalculateRevenueByTimePeriod([FromBody] TotalPriceDate priceDate)
    {
        return Ok(_orderService.CalculateRevenueByTimePeriod(priceDate.StartDate, priceDate.EndDate));
    }


    private Account GetAccountLogin()
    {
        string tokenRequestHeader =  HttpContext.Request.Headers["Authorization"];
        var token = _tokenService.ExtractTokenFromHeader(tokenRequestHeader);
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity.Name;
        var account = _accountService.FindByUsername(username);
        return account;
    }
}