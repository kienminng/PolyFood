namespace Plyfood.ResponseEntity;

public class AuthenticationResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}