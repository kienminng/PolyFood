using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plyfood.Entity
{
    public class Order
    {
        [Key]
        public int Order_Id { get; set; }
        [Required(ErrorMessage ="Payment can not be null")]
        public int? Payment_Id { get; set; }
        [ForeignKey(nameof(Payment_Id))]
        public Payment? Payment { get; set; }
        public int? User_Id { get; set; }
        [ForeignKey(nameof(User_Id))]
        public User? User { get; set; }
        [Range(0,double.MaxValue,ErrorMessage ="Original phải lớn hơn 0")]
        public double Original_Price { get; set; }
        [Range(0,double.MaxValue,ErrorMessage ="Actual phải lớn hơn 0")]
        public double Actual_Price { get; set; }
        [Required(ErrorMessage = "name can not be null")]
        public string Full_name { get; set; } = string.Empty;
        [Required(ErrorMessage = "phone number can not be null")]
        [MinLength(11)]
        [MaxLength(13)]
        public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "Address can not be null")]
        public string Address { get; set; } = string.Empty;
        [Required(ErrorMessage = "Order status can not be null")]
        public int? Order_Status_Id { get; set; }
        [ForeignKey(nameof(Order_Status_Id))]
        public OrderStatus? Order_Status { get; set; }
        [Required]
        public DateTime? Created { get; set; } = DateTime.Now;
        public DateTime? Updated { get; set; }

        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
