using System.Text;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class AesKeyLoadup
    {
        private static bool Loaded = false;
        public static async Task<bool> LoadAesKey(string? value, bool is_path)
        {
            bool result = false;
            try
            {
                if (Loaded == true)
                {
                    if (value != null)
                    {
                        AesKeyModel? model = null;
                        if (is_path == false)
                        {
                            model = await JsonFormatter.JsonDeserialiser<AesKeyModel>(value);
                            if (model != null)
                            {
                                Shared.aes = new AesFileEncryption(model);
                                result = true;
                            }
                        }
                        else
                        {
                            using (FileStream fs = File.Open(value, FileMode.Open))
                            {
                                byte[] file_binary = new byte[fs.Length];
                                await fs.ReadAsync(file_binary);

                                string aes_str = Encoding.UTF8.GetString(file_binary);
                                model = await JsonFormatter.JsonDeserialiser<AesKeyModel>(value);
                                if (model != null)
                                {
                                    Shared.aes = new AesFileEncryption(model);
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return result;
        }
    }
}
