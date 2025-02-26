namespace ThetaFTP.Shared.Models
{
    public class ServerPayloadModel
    {
        public enum Status
        {
            Success = 200,
            InternalServerError = 500
        }
        public Status status {  get; set; }
        public string? content { get; set; }
        public string? response_message { get; set; } = "Internal server error";
    }
}
