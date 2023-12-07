using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plyfood.Context;
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
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public OrderController(IOrderService orderService,AppDbContext context,ITokenService tokenService)
    {
        _orderService = orderService;
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin,User")]
    public IActionResult Create([FromBody] OrderCreatingForm creatingForm)
    {
        
        return Ok(_orderService.Create(creatingForm, GetAccountLogin().Users.FirstOrDefault().User_Id));
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
    public IActionResult CalculateRevenueByTimePeriod([FromQuery] DateTime started,[FromQuery] DateTime end)
    {
        return Ok(_orderService.CalculateRevenueByTimePeriod(started,end));
    }


    private Account GetAccountLogin()
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