using System.ComponentModel.DataAnnotations;
using Plyfood.Entity;

namespace Plyfood.Dto.Order;

public class OrderCreatingForm
{
    public string FullName { get; set; }
    [Required(ErrorMessage = "phone number can not be null")]
    [MinLength(11)]
    [MaxLength(13)]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "Address can not be null")]
    public string Address { get; set; }
    public List<OrderDetailCreatingDto> OrderDetails { get; set; }

   
}