namespace ThetaFTP.Shared.Classes
{
    using System.Security.Cryptography;
    using System.Text;

    public class LogInSessionKeyGenerator
    {
        private static char[] numbers = new char[10] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
        private static char[] symbols = new char[31] {'~', '`', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '_', '+', '=', '[', '{', ']', '}', '\\', '|', ';', ':', '\'', '"', ',', '<', '.', '>', '/', '?'};
        private static char[] letters = new char[26] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};


        public static Task<string> GenerateKey()
        {
            StringBuilder key = new StringBuilder();

            for (int i = 0; i < 10; i++)
            {
                int random = RandomNumberGenerator.GetInt32(1, 10);
                if (random <= 3)
                {
                    int random_index = RandomNumberGenerator.GetInt32(0, numbers.Length);
                    key?.Append(numbers[random_index].ToString());
                }
                else if (random <= 6)
                {
                    int random_index = RandomNumberGenerator.GetInt32(0, symbols.Length);
                    key?.Append(symbols[random_index].ToString());
                }
                else
                {
                    int random_index = RandomNumberGenerator.GetInt32(0, letters.Length);
                    key?.Append(letters[random_index].ToString());
                }
            }

            return Task.FromResult(key.ToString());
        }
    }
}
