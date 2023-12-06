using System.ComponentModel.DataAnnotations;

namespace Plyfood.Entity
{
    public class Payment
    {
        [Key]
        public int Payment_Id { get; set; }
        [Required]
        public string Payment_Method { get; set; } = string.Empty;
        [Required]
        [Range(0, 1)]
        public int Status { get; set; } = 0;
        [Required]
        public DateTime Create_At { get; set; } = DateTime.Now;
        public DateTime? Update_At { get; set; }
        public List<Order>? Orders { get; set; }
        
    }
}
