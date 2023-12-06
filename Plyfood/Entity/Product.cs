using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plyfood.Entity
{
    public class Product
    {
        [Key]
        public int Product_Id { get; set; } 
        public int? ProductType_Id { get; set; }
        [ForeignKey(nameof(ProductType_Id))]
        public ProductType? ProductType { get; set; }
        [Required(ErrorMessage ="Product name can not be null")]
        public string Name_Product { get; set; } = string.Empty;
        [MinLength(1)]
        public double Price { get; set; }
        [Required(ErrorMessage ="Imge can not be null")]
        public string Avatar_Image_Product { get; set; } 
        public string Title { get; set; } = string.Empty;
        public int? Discount { get; set; }
        public int? Status { get; set; }
        public int? Number_Of_View { get; set; }    
        public DateTime Create_At { get; set; } = DateTime.Now;
        public DateTime? Update_At { get;set; }
        public List<ProductReview>? Reviews { get; set; }
        public List<CartItem>? CartItems { get; set; }
        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
