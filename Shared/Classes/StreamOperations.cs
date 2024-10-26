using Org.BouncyCastle.Utilities.Zlib;
using System.Buffers;

namespace ThetaFTP.Shared.Classes
{
    public class StreamOperations
    {
        public static async Task<bool> ReadAsync(Stream input_stream, long file_size, Stream output_stream, int buffer_size, int buffer_count_flush, CancellationToken cancellation)
        {
            bool result = false;
            double timeout = GetTimeout();

            IMemoryOwner<byte> contingent_memory_buffer = MemoryPool<byte>.Shared.Rent(buffer_size);

            DateTime start = DateTime.Now;

            try
            {

                while (input_stream.CanRead == true && file_size > 0)
                {

                    if (cancellation.IsCancellationRequested == false)
                    {
                        if ((DateTime.Now - start) >= TimeSpan.FromMicroseconds(timeout))
                        {
                            start = DateTime.Now;

                            int bytes_read = await input_stream.ReadAsync(contingent_memory_buffer.Memory.Slice(0, buffer_size));
                            if (bytes_read > 0)
                            {
                                await output_stream.WriteAsync(contingent_memory_buffer.Memory.Slice(0, bytes_read));

                                if (output_stream.Length >= buffer_size * buffer_count_flush)
                                {
                                    await output_stream.FlushAsync();
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
                        return false;
                    }
                }

                if (output_stream.Length > 0)
                    await output_stream.FlushAsync();

                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                contingent_memory_buffer.Dispose();
            }

            return result;
        }
        

        private static double GetTimeout()
        {
            double divider = 1000;
            double? timeout = divider / (Shared.config?.ReadAndWriteOperationsPerSecond / divider);
            if (timeout == null)
                timeout = 2500;
            return (double)timeout;
        }
    }
}
