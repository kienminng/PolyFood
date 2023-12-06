namespace Plyfood.Helper;

public class Status
{
    public string? Invalid { get; set; }
    public string? BadGateway { get; set; }
    public string? ServiceUnavailable { get; set; }
    public string? BadRequest { get; set; }
    public string? NotFound { get; set; }
    public string? Authentication { get; set; }
    public string? PaymentRequired { get; set; }
    public string? Forbidden { get; set; }
    public string? Ok { get; set; }
    public string? NoContent { get; set; }
}