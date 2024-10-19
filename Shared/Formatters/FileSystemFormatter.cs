using System.Collections.Concurrent;
using System.IO;
using System.Text;
using ThetaFTP.Shared.Classes;

namespace ThetaFTP.Shared.Formatters
{
    public class FileSystemFormatter:Shared
    {
        private static Dictionary<string, State> directories_operations = new Dictionary<string, State>();
        private static Dictionary<string, State> files_operations = new Dictionary<string, State>();

        public static bool IsValidPath(string path_name) => path_name[0] == '/' ? path_name.All(c => char.IsLetter(c) || char.IsNumber(c) || c == '_' || c == '-' || c == ' ' || c == '/') : false;
        public static bool IsValidFileName(string file_name) => file_name.All(c => char.IsLetter(c) || char.IsNumber(c) || c == '_' || c == '-' || c == ' ' || c == '.');
        public static bool IsValidDirectoryName(string file_name) => file_name.All(c => char.IsLetter(c) || char.IsNumber(c) || c == '_' || c == '-' || c == ' ' || c == '.');
        public static string? PathConverter(string? path) => OperatingSystem.IsWindows() == true ? path?.Replace('/', '\\') : path;
        public static bool CheckIfDirectoryOnDisk(string? path) => IsValidDiskPath(path) == false ? false : true;
        public static char PathSeparator() => OperatingSystem.IsWindows() == true ? '\\' : '/';
        public static bool IsValidDiskPath(string? path) => Directory.Exists(path);
        public static long GetAvailableSpace() => new DriveInfo(Path.GetPathRoot(new FileInfo(Environment.CurrentDirectory).FullName) ?? String.Empty).AvailableFreeSpace;
        public static string PathConverter(string? path, string? email) => new StringBuilder(Environment.CurrentDirectory).Append(PathSeparator()).Append("FTP_Server").Append(PathSeparator()).Append(email).Append(PathConverter(path)).ToString();
        public static string GetSourcePath(string? email) => new StringBuilder(Environment.CurrentDirectory).Append(PathSeparator()).Append("FTP_Server").Append(PathSeparator()).Append(email).ToString();
        public static string FullPathBuilder(string? path, string? item) => new StringBuilder(path).Append(item).ToString();
        public static string DatabaseKeyBuilder(string? email, string? item) => new StringBuilder(email).Append('/').Append(item).ToString();
        public static void DeleteFile(string path) => new FileInfo(path).Delete();
        public static void DeleteDirectory(string path) => new DirectoryInfo(path).Delete();
        public static bool IsFile(string path) => File.Exists(path) == true;
        public static bool IsDirectory(string path) => Directory.Exists(path) == true;
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

        public static string? GenerateFullFilePath(string? file_name, string? path) => new StringBuilder(path).Append(file_name).ToString();

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


        public static bool GetFileOperationState(string email, string path, State.Operation operation) 
        {
            State? state = new State();
            files_operations.TryGetValue(email, out state);

            FileState? _state = new FileState();
            state?.operations.TryGetValue(path, out _state);

            bool? res = _state?.GetState(operation);

            if (res != null)
                return (bool)res;
            else
                return false;
        }


        public static void SetFileOperationState(string email, string path, State.Operation operation, bool state)
        {
            State? _state = null;
            files_operations.TryGetValue(email, out _state);

            if (_state != null)
            {
                FileState? fileState = null;
                _state?.operations.TryGetValue(path, out fileState);
                fileState?.SetState(operation, state);
            }
            else
            {
                FileState? fileState = new FileState();
                fileState.SetState(operation, state);

                _state = new State();
                _state.operations.Add(path, fileState);

                files_operations.Add(email, _state);
            }
        }

        public static bool GetDirectoryOperationState(string email, string path, State.Operation operation)
        {
            State? state = new State();
            directories_operations.TryGetValue(email, out state);

            FileState? _state = new FileState();
            state?.operations.TryGetValue(path, out _state);

            bool? res = _state?.GetState(operation);

            if (res != null)
                return (bool)res;
            else
                return false;
        }


        public static void SetDirectoryOperationState(string email, string path, State.Operation operation, bool state)
        {
            State? _state = null;
            directories_operations.TryGetValue(email, out _state);

            if (_state != null)
            {
                FileState? fileState = null;
                _state?.operations.TryGetValue(path, out fileState);
                fileState?.SetState(operation, state);
            }
            else
            {
                FileState? fileState = new FileState();
                fileState.SetState(operation, state);

                _state = new State();
                _state.operations.Add(path, fileState);

                directories_operations.Add(email, _state);
            }
        }


        public static bool ValidateFileOperation(string email, string path, State.Operation operation)
        {
            State? _state = null;
            files_operations.TryGetValue(email, out _state);

            if (_state != null)
            {
                FileState? fileState = null;
                _state?.operations.TryGetValue(path, out fileState);


            }
            else
            {
                FileState? fileState = new FileState();
            }

            return true;
        }

        public static bool ValidateDirectoryOperation(string email, string path, State.Operation operation)
        {

            return true;
        }


        public static bool FindRelocationFileOperation(string? email, string? path)
        {
            bool found = false;

            return found;
        }




        public static bool FindDeleteFileOperation(string? email, string? path)
        {
            bool found = false;

            return found;
        }



        public static bool FindRelocationDirectoryOperation(string? email, string? path)
        {
            bool found = false;

            return found;
        }



        public static bool FindDeleteDirectoryOperation(string? email, string? path)
        {
            bool found = false;

            return found;
        }




        public static void CreateLogsDir() => Directory.CreateDirectory(new StringBuilder(Environment.CurrentDirectory).Append(PathSeparator()).Append("ServerLogs").ToString());
    }
}
