﻿using System.Collections.Concurrent;
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
        public static string FullPathBuilder(string? path, string? item) => path?[path.Length - 1] == PathSeparator() ? new StringBuilder(path).Append(item).ToString() : new StringBuilder(path).Append(PathSeparator()).Append(item).ToString();
        public static void DeleteFile(string path) => new FileInfo(path).Delete();
        public static void DeleteDirectory(string path) => Directory.Delete(path, true);
        public static bool IsFile(string path) => File.Exists(path) == true;
        public static bool IsDirectory(string path) => Directory.Exists(path) == true;
        public static void RenameFile(string old_file, string new_file) 
        {
            FileInfo fileInfo = new FileInfo(old_file);
            FileInfo newFileInfo = new FileInfo(new_file);
            if (fileInfo?.Directory?.FullName == newFileInfo?.Directory?.FullName)
                fileInfo?.MoveTo(new_file);
        }
        public static void RenameDirectory(string old_directory, string new_directory)
        {
            DirectoryInfo fileInfo = new DirectoryInfo(old_directory);
            DirectoryInfo newFileInfo = new DirectoryInfo(new_directory);
            if (fileInfo?.Parent?.FullName == newFileInfo?.Parent?.FullName)
                fileInfo?.MoveTo(new_directory);
        }

        public static void MoveFile(string old_path, string new_path) => new FileInfo(old_path).MoveTo(new_path);

        public static void MoveDirectory(string old_path, string new_path) => new DirectoryInfo(old_path).MoveTo(new_path);

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

        public static string NavigateBackward(string? Path)
        {
            if (Path != null)
            {
                StringBuilder builder = new StringBuilder();
                int end = Path.LastIndexOf("/");

                if (end > 0)
                {
                    for (int start = 0; start < end; start++)
                        builder.Append(Path[start]);
                    return builder.ToString();
                }
                else
                    return "/";
            }
            else
            {
                return "/";
            }
        }

        public static Task<Tuple<bool, string?>> GetLastDir(string? Path)
        {
            bool result = false;
            string? last_dir = null;

            if (Path != null)
            {
                if (Path != "/")
                {
                    StringBuilder builder = new StringBuilder("/");
                    int index = Path.LastIndexOf('/');
                    if (index != 0)
                    {
                        int start = 0;
                        for (start = index - 1; start > 0; start--)
                            if (Path[start] == '/')
                                break;
                        for (int i = start + 1; i < index; i++)
                            builder.Append(Path[i]);
                    }

                    result = true;
                    last_dir = builder.ToString();
                }
                else
                {
                    result = false;
                }
            }

            return Task.FromResult(new Tuple<bool, string?>(result, last_dir));
        }

        public static string NavigateForward(string? Path, string? name)
        {
            if (Path?[Path.Length - 1] != '/')
                return new StringBuilder(Path).Append("/").Append(name).ToString();
            else
                return new StringBuilder(Path).Append(name).ToString();
        }

        public static void CreateLogsDir() => Directory.CreateDirectory(new StringBuilder(Environment.CurrentDirectory).Append(PathSeparator()).Append("ServerLogs").ToString());
    }
}
