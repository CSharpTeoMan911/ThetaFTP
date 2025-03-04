using System.Buffers;
using Serilog;

namespace ThetaFTP.Shared.Classes
{
    public class StreamOperations
    {
        public static async Task<bool> ReadAsync(Stream input_stream, long file_size, Stream output_stream, int buffer_size, int buffer_count_flush, CancellationToken cancellation)
        {
            bool result = false;
            double timeout = 1000* GetTimeout();

            try
            {
                using (IMemoryOwner<byte> contingent_memory_buffer = MemoryPool<byte>.Shared.Rent(buffer_size))
                {
                    DateTime start = DateTime.UtcNow;

                    while (input_stream.CanRead == true && file_size > 0)
                    {
                        DateTime end = DateTime.UtcNow;

                        if (cancellation.IsCancellationRequested == false)
                        {
                            if ((end - start).TotalMicroseconds >= timeout)
                            {
                                start = end;
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
                            result = false;
                            break;
                        }
                    }

                    if (output_stream.Length > 0)
                        await output_stream.FlushAsync();

                    result = true;
                }
            }
            catch(Exception e)
            {
                Log.Error(e, "Reading and writing FTP file error");
                result = false;
            }

            return result;
        }

        private static double GetTimeout() => Shared.configurations == null ? 4 : 1000 / Shared.configurations.WriteOperationsPerSecond;
    }
}
