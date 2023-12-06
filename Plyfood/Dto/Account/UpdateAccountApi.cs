namespace Plyfood.Dto.Account;

public class UpdateAccountApi
{
    public string Avatar { get; set; }
    public string? Phone { get; set;}
    public string Email { get; set;} = string.Empty;
    public string? Address { get; set;}
}