using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plyfood.Entity
{
    public class CartItem
    {
        [Key]
        public int Cart_Item_Id { get; set; }
        [Required(ErrorMessage ="Product can not be null")]
        public int? Product_Id { get; set; }
        [ForeignKey(nameof(Product_Id))]
        public Product? Product { get; set; }
        [Required(ErrorMessage ="cart can not be null")]
        public int Cart_Id { get; set; }
        [ForeignKey(nameof(Cart_Id))]
        public Cart? Cart { get; set; }
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; } = 0;
        [Required]
        public DateTime Create_At { get; set; } = DateTime.Now;
        public DateTime? Update_At { get; set; }

    }
}
