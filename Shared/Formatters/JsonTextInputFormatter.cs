using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;

namespace ThetaFTP.Shared.Formatters
{
    public class JsonTextInputFormatter : TextInputFormatter
    {
        public JsonTextInputFormatter()
        {
            SupportedMediaTypes.Add("application/json");
            SupportedMediaTypes.Add("text/plain");
            SupportedMediaTypes.Add("text/json");

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            string text = String.Empty;

            Stream stream = new MemoryStream();
            try
            {
                await context.HttpContext.Request.Body.CopyToAsync(stream);

                byte[] text_bytes = new byte[stream.Length];
                await stream.ReadAsync(text_bytes, 0, text_bytes.Length);

                text = encoding.GetString(text_bytes);
            }
            finally
            {
                await stream.DisposeAsync();
            }



            return await InputFormatterResult.SuccessAsync(text);
        }
    }
}
