﻿namespace ThetaFTP.Shared.Models
{
    public class FtpModel
    {
        public string? email { get; set; }
        public string? file_name { get; set; }
        public string? new_name { get; set; }
        public string? path { get; set; }
        public string? new_path { get; set; }
        public long size { get; set; }
        public Stream? fileStream { get; set; }
        public CancellationToken operation_cancellation { get; set; }
    }
}
