namespace ThetaFTP.Shared.Models
{
    public class ValidationModel
    {
        public Shared.ValidationType validationType { get; set; }
        public string? email { get; set; }
        public string? code { get; set; }
    }
}
