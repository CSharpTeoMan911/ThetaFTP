using Microsoft.AspNetCore.Mvc.Formatters;

namespace ThetaFTP.Shared.Formatters
{
    public class StreamInputFormatter:InputFormatter
    {
        public StreamInputFormatter()
        {
            SupportedMediaTypes.Add("application/octet-stream");
            SupportedMediaTypes.Add("application/json");
            SupportedMediaTypes.Add("text/plain");
            SupportedMediaTypes.Add("text/json");
        }

        protected override bool CanReadType(Type type)
        {
            return typeof(Stream) == type;
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            Stream stream = new MemoryStream();
            await context.HttpContext.Request.Body.CopyToAsync(stream);
            return await InputFormatterResult.SuccessAsync(stream);
        }
    }
}
