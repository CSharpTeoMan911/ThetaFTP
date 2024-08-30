namespace ThetaFTP.Shared.Classes
{
    using System.Text;
    using ThetaFTP.Shared.Formatters;

    public class ServerLoggerService
    {
        private static string ErrorLogsFile = "ErrorLogs.txt";

        
        public static async Task WriteErrorLog(string error_message, Type class_name, string method_name)
        {
            try
            {
                FileSystemFormatter.CreateLogsDir();

                StringBuilder error_log = new StringBuilder(Environment.CurrentDirectory).Append("ServerLogs").Append(FileSystemFormatter.PathSeparator()).Append(DateTime.Now.ToString()).Append("_").Append(ErrorLogsFile);

                FileStream errorLogStream = File.OpenWrite(error_log.ToString());
                try
                {
                    error_log.Clear();

                    error_log.Append("Error name: ").Append(error_message).Append("\n");

                    error_log.Append("Class name: ").Append(class_name.Name).Append("\n");

                    error_log.Append("Method name: ").Append(method_name).Append("\n");

                    byte[] error_log_text_as_bytes = Encoding.UTF8.GetBytes(error_log.ToString());

                    await errorLogStream.WriteAsync(error_log_text_as_bytes, 0, error_log_text_as_bytes.Length);
                    await errorLogStream.FlushAsync();
                }
                catch
                {

                }
                finally
                {
                    await errorLogStream.DisposeAsync();
                }
            }
            catch { }
        }

        public static async Task DeleteErrorLogFile()
        {
            try
            {


            }
            catch
            {

            }
        }
    }
}
