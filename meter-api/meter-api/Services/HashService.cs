
using System.Security.Cryptography;
using System.Text;

namespace meter_api.Services;

public class HashService : IHashService
{
    public string GetHash(string rawText)
    {
        var bytes = Encoding.UTF8.GetBytes(rawText);
        var hashBytes = SHA256.HashData(bytes);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        return hash;
    }
}
