using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace ThetaFTP.Shared.Formatters
{
    public class GzipStreamOutputFormatter : OutputFormatter
    {
        public GzipStreamOutputFormatter()
        {
            SupportedMediaTypes.Add("application/octet-stream");
        }

        protected override bool CanWriteType(Type? type)
        {
            return typeof(GZipStream) == type || typeof(FileStreamResult) == type;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            await WriteResponseBodyAsync(context);
        }
    }
}
