namespace ThetaFTP.Shared.Models
{
    public class KeyGenModel
    {
        public bool generate_key { get; set; }
        public int key_in_bits { get; set; } = 256;
        public int block_size { get; set; } = 128;
    }
}
