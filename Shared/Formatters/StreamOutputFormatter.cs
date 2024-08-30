using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace ThetaFTP.Shared.Formatters
{
    public class StreamOutputFormatter : OutputFormatter
    {
        public StreamOutputFormatter()
        {
            SupportedMediaTypes.Add("application/octet-stream");
        }

        protected override bool CanWriteType(Type? type)
        {
            return typeof(Stream) == type || typeof(FileStreamResult) == type;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            await WriteResponseBodyAsync(context);
        }
    }
}
