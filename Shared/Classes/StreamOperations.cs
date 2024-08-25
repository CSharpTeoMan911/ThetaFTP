﻿using System.Buffers;

namespace ThetaFTP.Shared.Classes
{
    public class StreamOperations
    {
        public static async Task<bool> ReadAsync(Stream input_stream, long file_size, Stream output_stream, int buffer_size, int buffer_count_flush, CancellationToken cancellation)
        {
            bool result = false;

            IMemoryOwner<byte> contingent_memory_buffer = MemoryPool<byte>.Shared.Rent(buffer_size);

            DateTime start = DateTime.Now;
            Console.WriteLine("file size: " + file_size);

            try
            {
                while (input_stream.CanRead == true)
                {
                    if (cancellation.IsCancellationRequested == false)
                    {
                        if ((DateTime.Now - start) >= TimeSpan.FromMicroseconds(GetOperationsPerSecond()))
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
            catch (Exception E)
            {
                result = false;
            }
            finally
            {
                contingent_memory_buffer.Dispose();
            }

            return result;
        }

        private static int GetOperationsPerSecond()
        {
            int? operations_per_second = 1000 / (Shared.config?.ReadAndWriteOperationsPerSecond / 1000);
            if (operations_per_second == null)
                operations_per_second = 2500;
            return (int)operations_per_second;
        }
    }
}
