using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plyfood.Entity
{
    public class OrderDetail
    {
        [Key]
        public int Order_Detail_Id { get; set; }
        [Required]
        public int? Order_Id { get; set; }
        [ForeignKey(nameof(Order_Id))]
        public Order? Order { get; set; }
        [Required]
        public int? Product_Id { get; set; }
        [ForeignKey(nameof(Product_Id))]
        public Product? Product { get; set; }
        [Required]
        public double Price_Total { get; set; } = 0.0;
        [Required]
        public int Quantity { get; set; } = 0;
        public DateTime? Create_At { get; set; } = DateTime.Now;
        public DateTime? Update_At { get; set; } 
    }
}
