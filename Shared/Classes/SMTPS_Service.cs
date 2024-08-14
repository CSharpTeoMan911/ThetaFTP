namespace ThetaFTP.Shared.Classes
{
    using MailKit.Net.Smtp;
    using MailKit;
    using MimeKit;
    using ThetaFTP.Shared.Models;

    public class SMTPS_Service
    {
        public static async Task<bool> SendSMTPS(string? email, string? subject, string? message)
        {
            bool response = false;

            SmtpClient client = new SmtpClient();

            try
            {
                client.Timeout = 10000;

                ServerConfigModel config = new ServerConfigModel();

                if (Shared.config != null)
                    config = Shared.config;

                MimeMessage message_object = new MimeMessage();
                message_object.From.Add(new MailboxAddress("ThetaFTP", config.smtp_email));
                message_object.To.Add(new MailboxAddress(email, email));
                message_object.Subject = subject;
                message_object.Body = new TextPart("plain")
                {
                    Text = message
                };

                await client.ConnectAsync(config.smtp_server, config.smtp_port, config.smtp_use_ssl);

                await client.AuthenticateAsync(config.smtp_email, config.smtp_password);

                await client.SendAsync(message_object);

                await client.DisconnectAsync(true);

                response = true;
            }
            catch(Exception E)
            {
                Console.WriteLine(E.Message);
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
