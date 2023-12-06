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
    public IActionResult Create([FromBody] CreateProductForm productForm)
    {
        return Ok(_productService.CreateProduct(productForm));
    }

    [HttpGet("views")]
    public IActionResult View([FromQuery] int id)
    {
        return Ok(_productService.ProductView(id));
    }

    [HttpGet("FindById")]
    public IActionResult FindById([FromQuery] int id)
    {
        return Ok(_productService.FindById(id));
    }
}