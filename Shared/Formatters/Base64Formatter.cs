using System.Text;

namespace ThetaFTP.Shared.Formatters
{
    public class Base64Formatter
    {
        public static Task<string> FromUtf8ToBase64(string? input) => Task.FromResult(input != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(input)) : throw new Exception("Null value"));
        public static Task<string> FromBase64ToUtf8(string? input) => Task.FromResult(input != null ? Encoding.UTF8.GetString(Convert.FromBase64String(input)) : throw new Exception("Null value"));
    }
}
