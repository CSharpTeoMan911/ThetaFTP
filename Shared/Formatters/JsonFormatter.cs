﻿using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using ThetaFTP.Shared.Classes;


namespace ThetaFTP.Shared.Formatters
{
    public class JsonFormatter
    {
        private static DefaultContractResolver contractResolver = new DefaultContractResolver();
        private static JsonSerializer serializer = new JsonSerializer()
        {
            ContractResolver = contractResolver
        };

        public static Task<ReturnType?> JsonDeserialiser<ReturnType>(string? json)
        {
            object? return_item = null;

            try
            {
                if (json != null)
                    using (StringReader sr = new StringReader(json))
                    {
                        using (JsonReader reader = new JsonTextReader(sr))
                        {
                            return_item = serializer.Deserialize<ReturnType>(reader);
                        }

                        sr?.Dispose();
                    }
            }
            catch(Exception e)
            {
                Logging.Message(e, "Json serialisation error", "Json serialisation error", "JsonFormatter", "JsonDeserialiser", Logging.LogType.Error);
            }

            return Task.FromResult((ReturnType?)return_item);
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
            catch (Exception e)
            {
                Logging.Message(e, "Json deserialisation error", "Json deserialisation error", "JsonFormatter", "JsonSerialiser", Logging.LogType.Error);
            }

            return Task.FromResult(return_item);
        }
    }
}
