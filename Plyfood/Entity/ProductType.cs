using System.ComponentModel.DataAnnotations;

namespace Plyfood.Entity
{
    public class ProductType
    {
        [Key]
        public int Product_Type_Id { get; set; }
        [Required(ErrorMessage ="Product type name can not be null")]
        public string Name_Product_Type { get; set; } = string.Empty;
        [Required(ErrorMessage ="Image type product can not be null")]
        public string Image_Type_Product {  get; set; } = string.Empty;
        [Required]
        public DateTime Create_At { get; set; } = DateTime.Now;
        public DateTime? Update_At { get; set; }

        public List<Product>? Products { get; set;}
    }
}
