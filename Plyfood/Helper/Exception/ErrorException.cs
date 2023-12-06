namespace Plyfood.Helper.Exception;

public class ErrorException : System.Exception
{
    public string? Status { get; set; }
    public string? Message { get; set; }
    public ErrorException(string message) : base(message)
    {
    }

    public ErrorException(string message,string status)
    {
        Message = message;
        Status = status;
    }
}