using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plyfood.Entity
{
    public class Account
    {
        [Key]
        public int Account_Id { get; set; }
        [Required(ErrorMessage ="Username can not be null")]
        public string User_name { get; set; } 
        public string Avatar { get; set; }

        [Required(ErrorMessage ="Password can not be null")] 
        public string Password { get; set; }
        public int Status { get; set; } = 0;
        public int? Decentralization_Id { get; set; }
        [ForeignKey(nameof(Decentralization_Id))]
        public Decentralization? Decentralization { get; set; }
        public string ResestPasswordToken { get; set; } = string.Empty;
        public DateTime? ResetPasswordTokenExpiry { get; set; }

        public List<User>? Users { get; set; }
        public Token? Token { get; set; }
    }
}
