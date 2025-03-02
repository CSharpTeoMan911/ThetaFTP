using Serilog;
using System.Security.Cryptography;
using System.Text;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP
{
    public class FileEncryptionKeyGen
    {
        private static string key_json_file = "encryption_key_generation_file.json";

        private class KeyGenResult
        {
            public byte[]? Key { get; set; }
            public byte[]? IV { get; set; }
            public KeyGenerationResult generationResult { get; set; }
        }

        public enum Result
        {
            GeneratedKeyConfigFile,
            GeneratedKeys,
            None
        }

        public enum KeyGenerationResult
        {
            Successful,
            Unsccessful
        }

        public static async Task<Result> KeyGen()
        {
            Result generation_result = Result.None;
            string? serialised_json = null;
            try
            {
                using (FileStream fs = File.OpenRead(key_json_file))
                {
                    byte[] json_binary = new byte[fs.Length];
                    await fs.ReadAsync(json_binary, 0, json_binary.Length);
                    serialised_json = Encoding.UTF8.GetString(json_binary);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error reading encryption key configurations");
            }

            KeyGenModel? model = await JsonFormatter.JsonDeserialiser<KeyGenModel?>(serialised_json);

            if (model == null)
            {
                await GenerateKeyConfig(new KeyGenModel());
                Console.WriteLine(new StringBuilder("\n\n\n Generated file encryption key genertion configuration file at: \n ").Append(Environment.CurrentDirectory).Append(FileSystemFormatter.PathSeparator()).Append(key_json_file).Append("\n\n\n"));
                generation_result = Result.GeneratedKeyConfigFile;
            }
            else
            {
                if (model.generate_key == true)
                {
                    KeyGenResult result = GenerateKeys(model);

                    if (result.generationResult == KeyGenerationResult.Successful)
                    {
                        await GenerateEncryptionKeyFile(result);
                        model.generate_key = false;
                        await GenerateKeyConfig(model);

                        Console.WriteLine(new StringBuilder("\n\n\n Generated file encryption key at: \n ").Append(Environment.CurrentDirectory).Append(FileSystemFormatter.PathSeparator()).Append("aes_key.json"));
                    }
                    generation_result = Result.GeneratedKeys;
                }
            }

            return generation_result;
        }


        private static async Task GenerateKeyConfig(KeyGenModel? model)
        {
            using (FileStream fs = File.Open(key_json_file, FileMode.Create))
            {
                string? serialised_model = await JsonFormatter.JsonSerialiser(model);
                if (serialised_model != null)
                {
                    byte[] serialised_model_binary = Encoding.UTF8.GetBytes(serialised_model);
                    await fs.WriteAsync(serialised_model_binary);
                    await fs.FlushAsync();
                }
            }
        }


        private static KeyGenResult GenerateKeys(KeyGenModel? model)
        {
            KeyGenResult result = new KeyGenResult();

            try
            {
                if (model != null)
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Mode = CipherMode.CBC;
                        aes.KeySize = 256;
                        aes.BlockSize = 128;
                        aes.GenerateIV();
                        aes.GenerateIV();

                        string encryption_test = " Encryption test ";

                        using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                        {
                            using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                            {
                                using (MemoryStream msEncrypt = new MemoryStream())
                                {
                                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                                    {
                                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                                        {
                                            swEncrypt.Write(encryption_test);
                                        }
                                    }

                                    byte[] encrypted = msEncrypt.ToArray();

                                    using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                                    {
                                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                                        {
                                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                                            {
                                                string plaintext = srDecrypt.ReadToEnd();

                                                if (encryption_test == plaintext)
                                                {
                                                    result.generationResult = KeyGenerationResult.Successful;
                                                    Console.WriteLine("\n\n\n [ Encryption key validation successful ] \n ");
                                                    result.Key = aes.Key;
                                                    result.IV = aes.IV;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("\n\n\n [ Encryption key is corrupted ] \n ");
                                                    throw new Exception("Encryption keys is corrupted");
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch(Exception e)
            {
                result.generationResult = KeyGenerationResult.Unsccessful;
                Log.Error(e, "Error creating encryption key file");
            }

            return result;
        }

        private static async Task GenerateEncryptionKeyFile(KeyGenResult key)
        { 
            if (key?.Key != null && key?.IV != null)
            {
                string? serialised_key = await JsonFormatter.JsonSerialiser(new AesKeyModel()
                {
                    Key = key.Key,
                    IV = key.IV,
                });

                if (serialised_key != null)
                {
                    byte[] key_binary = Encoding.UTF8.GetBytes(serialised_key);

                    using (FileStream fs = File.Open("aes_key.json", FileMode.Create))
                    {
                        await fs.WriteAsync(key_binary);
                        await fs.FlushAsync();
                    }
                }
            }
        }
    }
}
