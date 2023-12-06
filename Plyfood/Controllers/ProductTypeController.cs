using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plyfood.Dto.ProductTypes;
using Plyfood.Service.IService;

namespace Plyfood.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductTypeController : Controller
{
    private readonly IProductTypeService _productTypeService;

    public ProductTypeController(IProductTypeService productTypeService)
    {
        _productTypeService = productTypeService;
    }

    [HttpGet("getAll")]
    [AllowAnonymous]
    public IActionResult GetAll()
    {
        return Ok(_productTypeService.GetAll());
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] ProductTypeCreateForm form)
    {
        return Ok(_productTypeService.Save(form));
    }

    [HttpPost("update")]
    public IActionResult Update([FromBody] ProductTypeUpdateForm updateForm)
    {
        return Ok(_productTypeService.Update(updateForm));
    }

    [HttpGet("FindByName")]
    [AllowAnonymous]
    public IActionResult FindByName([FromQuery] string name)
    {
        return Ok(_productTypeService.FindByName(name));
    }
    
}