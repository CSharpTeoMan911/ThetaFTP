using System.IO;
using System.Text;

namespace ThetaFTP.Shared.Formatters
{
    public class FileSystemFormatter
    {
        public static bool IsValidPath(string path_name) => path_name[0] == '/' ? path_name.All(c => char.IsLetter(c) || char.IsNumber(c) || c == '_' || c == '-' || c == ' ' || c == '/') : false;
        public static bool IsValidFileName(string file_name) => file_name.All(c => char.IsLetter(c) || char.IsNumber(c) || c == '_' || c == '-' || c == ' ' || c == '.');
        public static bool IsValidDirectoryName(string file_name) => file_name.All(c => char.IsLetter(c) || char.IsNumber(c) || c == '_' || c == '-' || c == ' ' || c == '.');
        public static string? PathConverter(string? path) => OperatingSystem.IsWindows() == true ? path?.Replace('/', '\\') : path;
        public static bool CheckIfDirectoryOnDisk(string? path) => IsValidDiskPath(path) == false ? false : true;
        public static char PathSeparator() => OperatingSystem.IsWindows() == true ? '\\' : '/';
        public static bool IsValidDiskPath(string? path) => Directory.Exists(path);
        public static long GetAvailableSpace() => new DriveInfo(Path.GetPathRoot(new FileInfo(Environment.CurrentDirectory).FullName) ?? String.Empty).AvailableFreeSpace;
        public static string PathConverter(string? path, string? email) => new StringBuilder(Environment.CurrentDirectory).Append(PathSeparator()).Append("FTP_Server").Append(PathSeparator()).Append(email).Append(PathConverter(path)).ToString();
        public static string FullPathBuilder(string? path, string? item) => new StringBuilder(path).Append(item).ToString();
        public static string DatabaseKeyBuilder(string? email, string? item) => new StringBuilder(email).Append('/').Append(item).ToString();
        public static string DirectoryNameCharacterReplacement(string? directory_name)
        {
            if (directory_name == null)
                directory_name = String.Empty;
            foreach (char c in directory_name)
                if (char.IsLetter(c) == false && char.IsNumber(c) == false && c != '_' && c != '-' && c != ' ' && c != '.')
                    directory_name = directory_name.Replace(c.ToString(), String.Empty);
            return directory_name;
        }
        public static string FileNameCharacterReplacement(string? file_name)
        {
            if (file_name == null)
                file_name = String.Empty;
            foreach (char c in file_name)
                if (char.IsLetter(c) == false && char.IsNumber(c) == false && c != '_' && c != '-' && c != ' ' && c != '.')
                    file_name = file_name.Replace(c.ToString(), String.Empty);
            return file_name;
        }

        public static bool CreateUserRootDir(string? email)
        {
            char path_separator = PathSeparator();

            StringBuilder path_builder = new StringBuilder(Environment.CurrentDirectory);
            path_builder.Append(path_separator);
            path_builder.Append("FTP_Server");
            path_builder.Append(path_separator);
            path_builder.Append(email);

            string user_dir = path_builder.ToString();

            if (IsValidDiskPath(user_dir) == false)
                try
                {
                    Directory.CreateDirectory(user_dir);
                }
                catch
                {
                    return false;
                }

            return true;
        }
    }
}
