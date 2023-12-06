using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plyfood.Entity
{
    public class ProductReview
    {
        [Key] 
        public int Product_Review_Id { get; set; }
        [Required(ErrorMessage ="Product can not be null")]
        public int? Product_Id { get; set; }
        [ForeignKey(nameof(Product_Id))] 
        public Product? Product { get; set; }
        [Required(ErrorMessage ="User can not be null")] 
        public int? User_Id { get; set; }
        [ForeignKey(nameof(User_Id))]
        public User? User { get; set; }
        public string Content_rated { get; set; } = string.Empty;
        [Range(1,10,ErrorMessage = " ponit in to 10  ")]
        public int? Ponit_Evaluation { get; set; }
        public string Content_Seen { get; set; } = string.Empty;
        [Range(1, 0, ErrorMessage = "status error")]
        public int Status { get; set; } = 0;
        [Required]
        public DateTime Create_At { get; set; } = DateTime.Now;
        public DateTime? Update_At { get; set; }
        
    }
}
