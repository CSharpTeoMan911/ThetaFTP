namespace ThetaFTP.Shared.Models
{
    public class FtpModel
    {
        public string? email { get; set; }
        public string? file_name { get; set; }
        public string? path { get; set; }
        public Stream? fileStream { get; set; }
    }
}
