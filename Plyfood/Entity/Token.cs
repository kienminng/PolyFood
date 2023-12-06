using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plyfood.Entity;

public class Token
{
    [Key]
    public int TokenId { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public int? AccountId { get; set; }
    [ForeignKey(nameof(AccountId))] 
    public Account? Account { get; set; }
}