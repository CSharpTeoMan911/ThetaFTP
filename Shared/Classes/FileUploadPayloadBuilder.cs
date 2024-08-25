using Microsoft.AspNetCore.Components.Forms;
using QueryParser;
using System.Net.Http.Headers;
using System.Text;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class FileUploadPayloadBuilder
    {
        public static async Task<Tuple<string, Stream, StreamContent?>> FileUpload(IBrowserFile file, string? path, string? log_in_session_key)
        {
            string uri = String.Empty;
            Stream stream = new MemoryStream();
            StreamContent? content = null;

            try
            {
                FileOperationMetadata metadata = new FileOperationMetadata();
                stream = file.OpenReadStream(524288000);

                if (stream.CanRead == true)
                {
                    metadata.path = path;
                    metadata.file_name = FileSystemFormatter.FileNameCharacterReplacement(file.Name);
                    metadata.key = log_in_session_key;
                    metadata.file_length = stream.Length;

                    StringBuilder builder = new StringBuilder("/files/insert?");
                    builder.Append(await QueryParsing.QueryParser(metadata));
                    uri = builder.ToString();

                    content = new StreamContent(stream);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                }
            }
            catch(Exception E) 
            {
            }

            return new Tuple<string, Stream, StreamContent?>(uri, stream, content);
        }
    }
}
