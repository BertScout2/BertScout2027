using System.Text;

namespace BertScout2027.Utilities;

public static class EncodeDecode
{
    public static string Base64Encode(string plainText)
    {
        // Convert the string to a byte array using UTF-8 encoding
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        // Convert the byte array to a Base64 string
        return Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        // Convert the Base64 string back to a byte array
        var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);

        // Convert the byte array back to a string using UTF-8 encoding
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
}
