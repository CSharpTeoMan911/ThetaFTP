namespace ThetaFTP.Shared.Models
{
    public class PayloadModel
    {
        public System.Net.HttpStatusCode StatusCode { get; set; } = System.Net.HttpStatusCode.InternalServerError;
        public string? result { get; set; } = "Internal server error";
        public object? payload { get; set; }
    }
}
