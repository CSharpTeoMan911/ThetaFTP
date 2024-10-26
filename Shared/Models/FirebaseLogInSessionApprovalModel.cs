namespace ThetaFTP.Shared.Models
{
    public class FirebaseLogInSessionApprovalModel
    {
        public string? code { get; set; }
        public string? key { get; set; }
        public long expiry_date { get; set; }
    }
}
