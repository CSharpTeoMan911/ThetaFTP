using Org.BouncyCastle.Tls.Crypto;
using System.Buffers;
using System.Security.Cryptography;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class AesFileEncryption
    {
        private readonly AesKeyModel? aesKey;

        public AesFileEncryption(AesKeyModel aesKey)
        {
            this.aesKey = aesKey;
        }

        public async Task<bool> EncryptFile(Stream input_stream, long file_size, Stream output_stream, int buffer_size, int buffer_count_flush, CancellationToken cancellation)
        {
            bool result = false;
            double timeout = GetTimeout();

            if (aesKey?.Key != null && aesKey?.IV != null)
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.KeySize = aesKey.KeySize;
                    aes.BlockSize = aesKey.BlockSize;
                    aes.Key = aesKey.Key;
                    aes.IV = aesKey.IV;

                    using (IMemoryOwner<byte> contingent_memory_buffer = MemoryPool<byte>.Shared.Rent(buffer_size))
                    {
                        using (ICryptoTransform aesEncryptor = aes.CreateEncryptor())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(output_stream, aesEncryptor, CryptoStreamMode.Write))
                            {
                                DateTime start = DateTime.UtcNow;

                                while (input_stream.CanRead == true && file_size > 0)
                                {
                                    DateTime end = DateTime.UtcNow;

                                    if (cancellation.IsCancellationRequested == false)
                                    {
                                        if ((end - start).TotalMicroseconds >= 1000 * timeout)
                                        {
                                            start = end;

                                            int bytes_read = await input_stream.ReadAsync(contingent_memory_buffer.Memory.Slice(0, buffer_size));
                                            if (bytes_read > 0)
                                            {
                                                await cryptoStream.WriteAsync(contingent_memory_buffer.Memory.Slice(0, bytes_read));
                                                if (output_stream.Length >= buffer_size * buffer_count_flush)
                                                {
                                                    await cryptoStream.FlushAsync();
                                                }
                                                file_size -= bytes_read;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        result = false;
                                        break;
                                    }
                                }

                                if (output_stream.Length > 0)
                                {
                                    await cryptoStream.FlushAsync();
                                    await cryptoStream.FlushFinalBlockAsync();
                                }

                                result = true;
                            }
                        }
                    }
                }
            }

            return result;
        }


        public Stream? DecryptFile(Stream input_stream)
        {
            if (aesKey?.Key != null && aesKey?.IV != null)
            {
                Aes aes = Aes.Create();

                aes.Mode = CipherMode.CBC;
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = aesKey.Key;
                aes.IV = aesKey.IV;

                return new CryptoStream(input_stream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            }

            return null;
        }

        private static double GetTimeout()
        {
            double? timeout = 1000 / Shared.configurations?.WriteOperationsPerSecond;
            return timeout == null ? 2500 : (double)timeout;
        }
    }
}
