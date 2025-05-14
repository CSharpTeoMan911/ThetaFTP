using Serilog;
using System.Text;

namespace ThetaFTP.Shared.Classes
{
    public class Logging
    {
        public enum LogType
        {
            Fatal,
            Error
        }

        public static void Init()
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("ServerErrorLogs.txt",
                rollingInterval: RollingInterval.Day,                rollOnFileSizeLimit: true,
                retainedFileTimeLimit: TimeSpan.FromDays((Shared.configurations == null ? 10 : Shared.configurations.logs_expiration_days)))
                .CreateLogger();
        }

        public static void Message(Exception error, string message, string context, string _class, string _method, LogType type)
        {
            string log = new StringBuilder("\nDate: ")
                .Append(DateTime.Now.ToString()).Append("\n")
                .Append("Error: ").Append(error.Message).Append("\n")
                .Append("Message: ").Append(message).Append("\n")
                .Append("Context: ").Append(context).Append("\n")
                .Append("Class: ").Append(_class).Append("\n")
                .Append("Method: ").Append(_method).Append("\n\n\n\n")
                .ToString();

            if(type == LogType.Error)
            {
                Log.Error(log);
            }
            else
            {
                Log.Fatal(log);
            }
        }

        public static void Message(string error, string message, string context, string _class, string _method, LogType type)
        {
            string log = new StringBuilder("\nDate: ")
                .Append(DateTime.Now.ToString()).Append("\n")
                .Append("Error: ").Append(error).Append("\n")
                .Append("Message: ").Append(message).Append("\n")
                .Append("Context: ").Append(context).Append("\n")
                .Append("Class: ").Append(_class).Append("\n")
                .Append("Method: ").Append(_method).Append("\n\n\n\n")
                .ToString();

            if (type == LogType.Error)
            {
                Log.Error(log);
            }
            else
            {
                Log.Fatal(log);
            }
        }

        public static async void FlushLogs() => await Log.CloseAndFlushAsync();
    }
}
