namespace ThetaFTP.Shared.Models
{
    public class PasswordUpdateModel
    {
        public string? new_password { get; set; }
        public string? log_in_session_key { get; set; }
        public string? email { get; set; }
    }
}
