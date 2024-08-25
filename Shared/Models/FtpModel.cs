namespace ThetaFTP.Shared.Models
{
    public class FtpModel
    {
        public string? email { get; set; }
        public string? file_name { get; set; }
        public string? path { get; set; }
        public long size { get; set; }
        public Stream? fileStream { get; set; }
        public Stream? ftp_operation_info_channel { get; set; }
        public CancellationToken operation_cancellation { get; set; }
    }
}
