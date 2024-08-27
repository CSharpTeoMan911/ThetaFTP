namespace ThetaFTP.Shared.Models
{
    public class DirectoryItem
    {
        public string? name { get; set; }
        public long size { get; set; }
        public bool isDirectory { get; set; }
    }
}
