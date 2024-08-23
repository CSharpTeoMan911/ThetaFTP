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
            StringBuilder text = new StringBuilder();

            Stream stream = context.HttpContext.Request.Body;
            try
            {
                while (stream.CanRead == true)
                {
                    byte[] text_bytes = new byte[1024];
                    int bytes_read = await stream.ReadAsync(text_bytes, 0, text_bytes.Length);

                    if (bytes_read > 0)
                    {
                        text.Append(encoding.GetString(text_bytes, 0, bytes_read));
                    }
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                await stream.DisposeAsync();
            }



            return await InputFormatterResult.SuccessAsync(text.ToString());
        }
    }
}
