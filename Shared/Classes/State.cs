using Swashbuckle.AspNetCore.SwaggerGen;

namespace ThetaFTP.Shared.Classes
{
    public class State
    {
        public enum Operation
        {
            Download,
            Delete,
            Rename,
            Relocate,
            Upload
        }
        public Dictionary<string, FileState> operations { get; set; } = new Dictionary<string, FileState>();
    }
}
