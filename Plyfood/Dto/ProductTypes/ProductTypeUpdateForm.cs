using System.ComponentModel.DataAnnotations;

namespace Plyfood.Dto.ProductTypes;

public class ProductTypeUpdateForm
{
    public int ProductTypeId { get; set; }
    [Required(ErrorMessage = "Product Type name can not be null")]
    public string? NameProductType { get; set; }
    public string? Image { get; set; }
}