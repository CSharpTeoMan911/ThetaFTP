﻿namespace ThetaFTP.Shared.Models
{
    public class FirebaseApprovalModel
    {
        public string? email { get; set; }
        public string? key { get; set; }
        public long expiry_date { get; set; }
    }
}