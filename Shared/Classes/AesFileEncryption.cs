using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class AesFileEncryption
    {
        private readonly AesKeyModel? aesKey;
        public AesFileEncryption(AesKeyModel aesKey)
        {
            this.aesKey = aesKey;
        }


    }
}
