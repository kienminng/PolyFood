using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plyfood.Dto.Prodct;
using Plyfood.Service.IService;

namespace Plyfood.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public IActionResult Create([FromBody] CreateProductForm productForm)
    {
        return Ok(_productService.CreateProduct(productForm));
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public IActionResult Update([FromBody] UpdateProductForm form)
    {
        return Ok(_productService.UpdateProduct(form));
    }

    [HttpGet("FindByName")]
    [AllowAnonymous]
    public IActionResult FindByName([FromQuery] string name)
    {
        return Ok(_productService.FindByName(name));
    }
    
    [HttpGet("views")]
    [AllowAnonymous]
    public IActionResult View([FromQuery] int id)
    {
        return Ok(_productService.ProductView(id));
    }

    [HttpGet("FindById")]
    [Authorize(Roles = "Admin")]
    public IActionResult FindById([FromQuery] int id)
    {
        return Ok(_productService.FindById(id));
    }
}