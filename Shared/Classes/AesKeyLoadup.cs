using Google.Protobuf.WellKnownTypes;
using System.Text;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class AesKeyLoadup
    {
        private static bool Loaded = false;

        public static async Task<bool> LoadAesKeyFromValue(string? value)
        {
            bool result = false;

            try
            {
                if (Loaded == false)
                {
                    if (value != null)
                    {
                        AesKeyModel? model = await JsonFormatter.JsonDeserialiser<AesKeyModel>(value);
                        if (model != null)
                        {
                            Shared.SetAes(new AesFileEncryption(model));
                            result = true;
                        }
                    }

                    Loaded = true;
                }
            }
            catch { }

            return result;
        }

        public static async Task<bool> LoadAesKeyFromFile(string? path)
        {
            bool result = false;
            try
            {
                if (Loaded == false)
                {
                    if (path != null)
                    {
                        using (FileStream fs = File.Open(path, FileMode.Open))
                        {
                            AesKeyModel? model = null;

                            byte[] file_binary = new byte[fs.Length];
                            await fs.ReadAsync(file_binary, 0, file_binary.Length);

                            string aes_str = Encoding.UTF8.GetString(file_binary);
                            model = await JsonFormatter.JsonDeserialiser<AesKeyModel>(aes_str);
                            if (model != null)
                            {
                                Shared.SetAes(new AesFileEncryption(model));
                                result = true;
                            }
                        }
                    }

                    Loaded = true;
                }
            }
            catch { }

            return result;
        }
    }
}
