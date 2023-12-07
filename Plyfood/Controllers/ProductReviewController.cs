using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plyfood.Context;
using Plyfood.Dto.Reviews;
using Plyfood.Entity;
using Plyfood.Service.Impl;
using Plyfood.Service.IService;

namespace Plyfood.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductReviewController : Controller
{
    private readonly IProductReviewService _productReviewService;
    private readonly ITokenService _tokenService;
    private readonly AppDbContext _context;

    public ProductReviewController(IProductReviewService productReviewService,ITokenService tokenService,AppDbContext context)
    {
        _productReviewService = productReviewService;
        _tokenService = tokenService;
        _context = context;
    }

    [HttpPost("create")]
    [Authorize]
    public IActionResult Create([FromBody] CreateReviewDto dto)
    {
        return Ok(_productReviewService.CreateReview(dto,GetAccountLogin()));
    }

    [HttpPost("update")]
    [Authorize]
    public IActionResult Update([FromBody] UpdateReviewDto dto)
    {
        return Ok(_productReviewService.UpdateReview(dto, GetAccountLogin()));
    }

    [HttpDelete("delete")]
    [Authorize]
    public IActionResult Delete([FromBody] int id)
    {
        return Ok(_productReviewService.DeleteReview(id, GetAccountLogin()));
    }

    [HttpGet("History")]
    [Authorize]
    public IActionResult History()
    {
        return Ok(_productReviewService.HistoryReviewsByUser(GetAccountLogin()));
    }
    
    private Account GetAccountLogin(){
        var headerToken = HttpContext.Request.Headers.Authorization;
        var token = _tokenService.ExtractTokenFromHeader(headerToken);
        var principal = _tokenService.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity.Name;
        var account = 
            _context.Accounts
                .Include(x=> x.Users)
                .ThenInclude(x=> x.Carts)
                .FirstOrDefault(x=> x.User_name == username);
        return account;
    }
}