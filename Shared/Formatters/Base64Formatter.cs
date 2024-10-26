using System.Text;

namespace ThetaFTP.Shared.Formatters
{
    public class Base64Formatter
    {
        public static string FromUtf8ToBase64(string? input) => input != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(input)) : throw new Exception("Null value");
        public static string FromBase64ToUtf8(string? input) => input != null ? Encoding.UTF8.GetString(Convert.FromBase64String(input)) : throw new Exception("Null value");
    }
}
