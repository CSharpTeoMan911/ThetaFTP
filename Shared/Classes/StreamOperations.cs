using System.Buffers;
using Serilog;

namespace ThetaFTP.Shared.Classes
{
    public class StreamOperations
    {
        public static async Task<bool> ReadAsync(Stream input_stream, long file_size, Stream output_stream, int buffer_size, int buffer_count_flush, CancellationToken cancellation)
        {
            bool result = false;
            double timeout = GetTimeout();

            IMemoryOwner<byte> contingent_memory_buffer = MemoryPool<byte>.Shared.Rent(buffer_size);
            DateTime start = DateTime.UtcNow;
            try
            {
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
            catch(Exception e)
            {
                Log.Error(e, "Reading and writing FTP file error");
                result = false;
            }
            finally
            {
                contingent_memory_buffer.Dispose();
            }

            GC.Collect();
            return result;
        }
        

        private static double GetTimeout()
        {
            double? timeout = 1000 / Shared.configurations?.ReadAndWriteOperationsPerSecond;
            return timeout == null ? 2500 : (double)timeout;
        }
    }
}
