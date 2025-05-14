using QueryParser;
using System.Text;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class DirectoryUploadPayloadBuilder
    {
        public static Task<string> DirectoryUpload(string? directory, string? path, string? log_in_session_key) => Task.FromResult(new StringBuilder("/directories/insert?").Append(QueryParsing.QueryParser(new DirectoryOperationMetadata()
        {
            path = path,
            directory_name = FileSystemFormatter.ItemNameCharacterReplacement(directory),
            key = log_in_session_key,
        })).ToString());
    }
}
