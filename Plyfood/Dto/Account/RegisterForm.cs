using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Plyfood.Dto.Account;

public class RegisterForm
{
    [NotNull]
    public string? UserName { get; set; }

    public string? Avatar { get; set; } = string.Empty;
    [NotNull]
    [MinLength(6)]
    public string? Password { get; set; }
    [EmailAddress]
    [NotNull]
    public string? Email { get; set; }
    [MinLength(10)]
    [MaxLength(13)]
    public string Phone { get; set; } = string.Empty; 
    public string Address { get; set; } = string.Empty;
    

}