namespace ThetaFTP
{
    using System.Buffers.Text;
    using System.Security.Cryptography;
    using System.Text;

    public class Sha512Hasher
    {
        private readonly string? salt;
        public Sha512Hasher(string? salt)
        {
            this.salt = salt;
        }

        public async Task<string> Hash(string? value)
        {
            if (salt != null && value != null)
            {
                StringBuilder hash_builder = new StringBuilder();
                hash_builder.Append(salt);
                hash_builder.Append(value);

                byte[] content = Encoding.UTF8.GetBytes(hash_builder.ToString());

                using (SHA512 hash = SHA512.Create())
                {
                    using (MemoryStream ms = new MemoryStream(content))
                    {
                        byte[] hash_bytes = await hash.ComputeHashAsync(ms);
                        value = Convert.ToBase64String(hash_bytes);
                    }
                }
            }
            else
            {
                throw new Exception("Internal server error");
            }

            return value;
        }
    }
}
