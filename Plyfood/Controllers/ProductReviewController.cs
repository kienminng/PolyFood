using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plyfood.Dto.Reviews;
using Plyfood.Service.Impl;

namespace Plyfood.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductReviewController : Controller
{
    private readonly ProductReviewService _productReviewService;

    public ProductReviewController(ProductReviewService productReviewService)
    {
        _productReviewService = productReviewService;
    }

    [HttpPost("create")]
    [Authorize]
    public IActionResult Create([FromBody] CreateReviewDto dto)
    {
        var username = HttpContext.User.FindFirst("Name").Value;
        return Ok(_productReviewService.CreateReview(dto,username));
    }

    [HttpPost("update")]
    [Authorize]
    public IActionResult Update([FromBody] UpdateReviewDto dto)
    {
        var username = HttpContext.User.FindFirst("Name").Value;
        return Ok(_productReviewService.UpdateReview(dto, username));
    }

    [HttpDelete("delete")]
    [Authorize]
    public IActionResult Delete([FromBody] int id)
    {
        var username = HttpContext.User.FindFirst("Name").Value;
        return Ok(_productReviewService.DeleteReview(id, username));
    }

    [HttpGet("History")]
    [Authorize]
    public IActionResult History()
    {
        var username = HttpContext.User.FindFirst("Name").Value;
        return Ok(_productReviewService.HistoryReviewsByUser(username));
    }
}