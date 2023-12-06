namespace Plyfood.Dto.Account;

public class ResetPasswordApi
{
    public string? Username { get; set; }
    public string? RestPasswordToken { get; set; }
    public string? NewPassword { get; set; }
}