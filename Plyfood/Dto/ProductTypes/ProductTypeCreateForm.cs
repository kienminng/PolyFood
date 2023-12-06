using System.ComponentModel.DataAnnotations;

namespace Plyfood.Dto.ProductTypes;

public class ProductTypeCreateForm
{
    [Required(ErrorMessage = "Product Type name can not be null")]
    public string? NameProductType { get; set; }
    public string? Image { get; set; }
}