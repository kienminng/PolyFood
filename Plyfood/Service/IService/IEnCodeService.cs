namespace Plyfood.Service.IService;

public interface IEnCodeService
{
    byte[] EncryptString(string plainText, byte[] key);
    string DecryptString(byte[] cipherText, byte[] key);
    String HmacSHA512(string key, String inputData);

    string GetIpAddress(HttpContext httpContext);
}