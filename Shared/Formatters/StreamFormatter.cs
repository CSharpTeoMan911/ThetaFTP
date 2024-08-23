using Microsoft.AspNetCore.Mvc.Formatters;

namespace ThetaFTP.Shared.Formatters
{
    public class StreamFormatter : InputFormatter
    {
        public StreamFormatter()
        {
            SupportedMediaTypes.Add("application/octet-stream");
        }

        protected override bool CanReadType(Type type)
        {
            return typeof(Stream) == type;
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            return await InputFormatterResult.SuccessAsync(context.HttpContext.Request.Body);
        }
    }
}
