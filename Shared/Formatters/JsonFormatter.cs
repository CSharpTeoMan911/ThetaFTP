﻿using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace ThetaFTP.Shared.Formatters
{
    public class JsonFormatter
    {
        private static DefaultContractResolver contractResolver = new DefaultContractResolver();
        private static JsonSerializer serializer = new JsonSerializer()
        {
            ContractResolver = contractResolver
        };

        public static Task<ReturnType?> JsonDeserialiser<ReturnType>(string json)
        {
            ReturnType? return_item = default;

            try
            {
                using (StringReader sr = new StringReader(json))
                {
                    using (JsonReader reader = new JsonTextReader(sr))
                    {
                        return_item = serializer.Deserialize<ReturnType>(reader);
                    }

                    sr?.Dispose();
                }
            }
            catch { }

            return Task.FromResult((ReturnType?)(object?)return_item);
        }


        public static Task<string?> JsonSerialiser<InsertType>(InsertType? item)
        {
            string? return_item = null;

            try
            {
                using (StringWriter tw = new StringWriter())
                {
                    using (JsonWriter writer = new JsonTextWriter(tw))
                    {
                        writer.Formatting = Formatting.Indented;
                        serializer.Serialize(writer, item);
                        return_item = tw.ToString();
                    }

                    tw?.Dispose();
                }
            }
            catch { }

            return Task.FromResult(return_item);
        }
    }
}
