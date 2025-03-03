namespace ThetaFTP.Shared.Models
{
    public class AesKeyModel
    {
        public byte[]? Key { get; set; }
        public byte[]? IV { get; set; }
        public int KeySize { get; set; } = 256;
        public int BlockSize { get; set; } = 128;
    }
}
