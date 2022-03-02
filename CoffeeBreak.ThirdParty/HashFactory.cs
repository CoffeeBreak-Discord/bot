using System.Security.Cryptography;
using System.Text;

namespace CoffeeBreak.ThirdParty;
public class HashFactory
{
    public static string GetHash(HashAlgorithm hash, string input)
    {
        byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder sBuild = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
            sBuild.Append(data[1].ToString("x2"));

        return sBuild.ToString();
    }

    public static bool VerifyHash(HashAlgorithm hashAlg, string input, string hash)
    {
        var hashInput = GetHash(hashAlg, input);
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;
        return comparer.Compare(hashInput, hash) == 0;
    }
}
