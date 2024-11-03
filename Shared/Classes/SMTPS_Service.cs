namespace ThetaFTP.Shared.Classes
{
    using MailKit.Net.Smtp;
    using MailKit;
    using MimeKit;
    using ThetaFTP.Shared.Models;
    using static Org.BouncyCastle.Math.EC.ECCurve;

    public class SMTPS_Service:Shared
    {
        public static bool SendSMTPS(string? email, string? subject, string? message)
        {
            bool response = false;

            SmtpClient client = new SmtpClient();

            try
            {
                if (configurations != null && credentials != null)
                {
                    client.Timeout = 10000;

                    MimeMessage message_object = new MimeMessage();
                    message_object.From.Add(new MailboxAddress("ThetaFTP", credentials.smtp_email));
                    message_object.To.Add(new MailboxAddress(email, email));
                    message_object.Subject = subject;
                    message_object.Body = new TextPart("plain")
                    {
                        Text = message
                    };
                    client.Connect(configurations.smtp_server, configurations.smtp_port, configurations.smtp_use_ssl);
                    client.Authenticate(credentials.smtp_email, credentials.smtp_password);

                    client.Send(message_object);

                    client.Disconnect(true);

                    response = true;
                }
                else
                {
                    response = false;
                }
            }
            catch
            { 
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
