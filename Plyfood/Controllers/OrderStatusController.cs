using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plyfood.Entity;
using Plyfood.Service.IService;

namespace Plyfood.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrderStatusController : Controller
{
    private readonly IOrderStatusService _orderStatusService;

    public OrderStatusController(IOrderStatusService orderStatusService)
    {
        _orderStatusService = orderStatusService;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public IActionResult Create([FromBody] string name)
    {
        return Ok(_orderStatusService.Create(name));
    }

    [HttpPost("Update")]
    [Authorize(Roles = "Admin")]
    public IActionResult Update([FromBody] OrderStatus orderStatus)
    {
        return Ok(_orderStatusService.Update(orderStatus));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAll()
    {
        return Ok(_orderStatusService.GetAll());
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete([FromBody] int id)
    {
        return Ok(_orderStatusService.Delete(id));
    }
}