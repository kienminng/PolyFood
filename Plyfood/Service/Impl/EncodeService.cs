using System.Security.Cryptography;
using System.Text;
using Plyfood.Service.IService;

namespace Plyfood.Service.Impl;

public class EncodeService : IEnCodeService
{
    public EncodeService()
    {
    }

    public byte[] EncryptString(string plainText, byte[] key)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.Padding = PaddingMode.PKCS7;

            // Sử dụng CBC (Cipher Block Chaining) mode và IV (Initialization Vector) ngẫu nhiên
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.GenerateIV();

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Ghi dữ liệu vào CryptoStream
                        swEncrypt.Write(plainText);
                    }
                }
                
                byte[] result = new byte[aesAlg.IV.Length + msEncrypt.ToArray().Length];
                Array.Copy(aesAlg.IV, result, aesAlg.IV.Length);
                Array.Copy(msEncrypt.ToArray(), 0, result, aesAlg.IV.Length, msEncrypt.ToArray().Length);

                return result;
            }
        }
    }

    public string DecryptString(byte[] cipherText, byte[] key)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.Padding = PaddingMode.PKCS7;

            // Sử dụng CBC (Cipher Block Chaining) mode và IV (Initialization Vector) từ dữ liệu mã hoá
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.IV = cipherText.Take(16).ToArray(); 

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText.Skip(16).ToArray()))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
    
    
    public String HmacSHA512(string key, String inputData)
    {
        var hash = new StringBuilder();
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            byte[] hashValue = hmac.ComputeHash(inputBytes);
            foreach (var theByte in hashValue)
            {
                hash.Append(theByte.ToString("x2"));
            }
        }

        return hash.ToString();
    }

    public string GetIpAddress(HttpContext httpContext)
    {
        string ipAddress = httpContext.Connection.RemoteIpAddress.ToString();
        return ipAddress;
    }
}