using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plyfood.Entity
{
    public class Cart
    {
        [Key]
        public int Cart_Id { get; set; }
        [Required]
        public int? User_Id { get; set; }
        [ForeignKey(nameof(User_Id))]
        public User? User { get; set; }
        [Required]
        public DateTime? Create_At { get; set; } = DateTime.Now;
        public DateTime? Update_At { get; set; }

        public List<CartItem>? Items { get; set;}
    }
}
