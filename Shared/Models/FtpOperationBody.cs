namespace ThetaFTP.Shared.Models
{
    public class FtpOperationBody
    {
        public Stream? fileStream { get; set; }
        public Stream? progressStream { get; set; }
    }
}
