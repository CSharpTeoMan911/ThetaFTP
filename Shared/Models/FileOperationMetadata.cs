namespace ThetaFTP.Shared.Models
{
    public class FileOperationMetadata
    {
        public string? key { get; set; }
        public string? file_name { get; set; }
        public string? new_name { get; set; }
        public string? path { get; set; }
        public string? new_path { get; set; }
        public long file_length { get; set; }
        public Stream? ftp_operation_info_channel { get; set; }
    }
}
