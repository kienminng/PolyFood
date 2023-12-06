namespace Plyfood.Helper.ResponseMessage;

public class AccountMessage
{
    public string? UserIsNotExist { get; set; }
    public string? WrongPassword { get; set; }
    public string? EmailIsNotExist { get; set; }
}