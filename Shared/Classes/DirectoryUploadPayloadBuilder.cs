using QueryParser;
using System.Text;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class DirectoryUploadPayloadBuilder
    {
        public static async Task<string?> DirectoryUpload(string? directory, string? path, string? log_in_session_key) => new StringBuilder("/directories/insert?").Append(await QueryParsing.QueryParser(new DirectoryOperationMetadata()
        {
            path = path,
            directory_name = FileSystemFormatter.FileNameCharacterReplacement(directory),
            key = log_in_session_key,
        })).ToString();
    }
}
