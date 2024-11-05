namespace ThetaFTP.Shared.Models
{
    public class PasswordUpdateValidationModel
    {
        public string? code { get; set; }
        public string? new_password { get; set; }
    }
}
