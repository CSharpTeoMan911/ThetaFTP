namespace HallRentalSystem.Classes.StructuralAndBehavioralElements.Formaters
{
    using System.Buffers.Text;
    using System.Security.Cryptography;
    using System.Text;

    public class Sha512Hasher
    {
        private const string salt = "djahwsDKLJASEGJVHBSERJ23029Q04YTIFPWOLE;";
        public static Task<Tuple<string, Type>> Hash(string? password)
        {
            StringBuilder hash_builder = new StringBuilder();
            hash_builder.Append(salt);
            hash_builder.Append(password);

            byte[] content = Encoding.UTF8.GetBytes(hash_builder.ToString());

            SHA512 hash = SHA512.Create();

            try
            {
                byte[] hash_bytes = hash.ComputeHash(content);
                password = Convert.ToBase64String(hash_bytes);
            }
            catch
            {
                hash?.Dispose();
                return Task.FromResult(new Tuple<string, Type>("Internal server error", typeof(Exception)));
            }
            finally
            {
                hash?.Dispose();
            }

            return Task.FromResult(new Tuple<string, Type>(password, typeof(string)));
        }
    }
}
