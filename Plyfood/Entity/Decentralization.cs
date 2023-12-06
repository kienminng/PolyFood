using System.ComponentModel.DataAnnotations;

namespace Plyfood.Entity
{
    public class Decentralization
    {
        [Key]
        public int Decentralization_Id { get; set; }
        [Required]
        public string Authority_name { get; set; }
        [Required]
        public DateTime Created_At { get; set; }
        public DateTime? Update_At { get; set; }

        public List<Account> Accounts { get; set; }
    }
}
