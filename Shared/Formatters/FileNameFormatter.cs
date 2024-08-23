namespace ThetaFTP.Shared.Formatters
{
    public class FileNameFormatter
    {
        public static string CharacterReplacement(string file_name)
        {
            foreach (char c in file_name)
                if (char.IsLetter(c) == false && char.IsNumber(c) == false && c != '_' && c != '-' && c != ' ' && c != '.')
                    file_name = file_name.Replace(c.ToString(), String.Empty);
            return file_name;
        }
    }
}
