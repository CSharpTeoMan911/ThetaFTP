namespace ThetaFTP.Shared.Classes
{
    using MailKit.Net.Smtp;
    using MimeKit;
    using Serilog;

    public class SMTPS_Service:Shared
    {
        public static bool SendSMTPS(string? email, string? subject, string? message)
        {
            bool response = false;

            SmtpClient client = new SmtpClient();

            try
            {
                if (configurations != null)
                {
                    client.Timeout = 10000;

                    MimeMessage message_object = new MimeMessage();
                    message_object.From.Add(new MailboxAddress("ThetaFTP", configurations.smtp_email));
                    message_object.To.Add(new MailboxAddress(email, email));
                    message_object.Subject = subject;
                    message_object.Body = new TextPart("plain")
                    {
                        Text = message
                    };
                    client.Connect(configurations.smtp_server, configurations.smtp_port, configurations.smtp_use_ssl);
                    client.Authenticate(configurations.smtp_email, configurations.smtp_password);

                    client.Send(message_object);

                    client.Disconnect(true);

                    response = true;
                }
                else
                {
                    response = false;
                }
            }
            catch(Exception e)
            {
                Logging.Message(e, "Error sending SMTP request", "Check if the credentials and SMTP server address are valid", "SMTPS_Service", "SendSMTPS", Logging.LogType.Error);
                response = false;
            }
            finally
            {
                client.Dispose();
            }

            return response;
        }
    }
}
