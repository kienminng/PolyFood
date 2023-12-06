using System.ComponentModel.DataAnnotations;

namespace Plyfood.Dto.Account;

public class AuthenRequest
{
    [Required(ErrorMessage = "username not null")]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "password not null")]
    public string Password { get; set; } = string.Empty;
}