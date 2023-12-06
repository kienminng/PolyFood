using System.ComponentModel.DataAnnotations;

namespace Plyfood.Dto.Order;

public class OrderUpdateForm
{
    public int OrderId { get; set; }
    public double OriginalPrice { get; set; }
    public double ActualPrice { get; set; }
    public string FullName { get; set; }
    [Required]
    public int PhoneNumber { get; set; }
    [Required]
    public string Address { get; set; }
}