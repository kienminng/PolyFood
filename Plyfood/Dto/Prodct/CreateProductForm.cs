using System.ComponentModel.DataAnnotations;

namespace Plyfood.Dto.Prodct;

public class CreateProductForm
{
    public string? NameProduct { get; set; }
    [Required]
    public int? ProductTypeId { get; set; }
    public float Price { get; set; }
    public string? AvatarImageProduct { get; set; }
    public string? Title { get; set; }
    public int Discount { get; set; }
}