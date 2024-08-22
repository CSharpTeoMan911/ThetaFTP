using MySqlConnector;
using System.Text;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    public class FtpDatabaseController : CRUD_Interface<FtpModel, FtpModel, FtpModel, FtpModel>
    {
        public Task<string?> Delete(FtpModel? value)
        {
            throw new NotImplementedException();
        }

        public Task<string?> Get(FtpModel? value)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> Insert(FtpModel? value)
        {
            string? result = "Internal server error";


            if (value?.fileStream != null)
            {
                if (value?.file_name != null)
                {
                    if (value?.path != null)
                    {
                        if (IsValidFileName(value.file_name) == true)
                        {
                            if (IsValidPath(value.path) == true)
                            {
                                if (CreateUserRootDir(value?.email))
                                {
                                    string converted_path = PathConverter(value?.path, value?.email);

                                    if (IsValidDiskPath(converted_path) == true)
                                    {
                                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();
                                        try
                                        {
                                            MySqlCommand insert_file_command = connection.CreateCommand();
                                            try
                                            {
                                                StringBuilder file_name_builder = new StringBuilder(value?.email);
                                                file_name_builder.Append("/");
                                                file_name_builder.Append(value?.file_name);

                                                Console.WriteLine($"Path: {converted_path}");

                                                try
                                                {
                                                    StringBuilder file_path = new StringBuilder(converted_path);
                                                    file_path.Append(value?.file_name);

                                                    FileStream file_stream = File.OpenWrite(file_path.ToString());
                                                    try
                                                    {
                                                        if (value != null)
                                                        {
                                                            await value.fileStream.CopyToAsync(file_stream);
                                                            await file_stream.FlushAsync();

                                                            insert_file_command.CommandText = "INSERT INTO Files VALUES(@File_Name, @File_Path, @Email)";
                                                            insert_file_command.Parameters.AddWithValue("File_Name", file_name_builder.ToString());
                                                            insert_file_command.Parameters.AddWithValue("File_Path", value?.path);
                                                            insert_file_command.Parameters.AddWithValue("Email", value?.email);

                                                            await insert_file_command.ExecuteNonQueryAsync();

                                                            result = "File upload successful";
                                                        }
                                                        else
                                                        {
                                                            result = "Internal server error";
                                                        }
                                                    }
                                                    catch (Exception E)
                                                    {
                                                        Console.WriteLine($"Error: {E.Message}");
                                                    }
                                                    finally
                                                    {
                                                        await file_stream.DisposeAsync();
                                                    }
                                                }
                                                catch (Exception E)
                                                {
                                                    Console.WriteLine($"Error: {E.Message}");
                                                    result = "Internal server error";
                                                }
                                            }
                                            finally
                                            {
                                                await insert_file_command.DisposeAsync();
                                            }
                                        }
                                        finally
                                        {
                                            await connection.DisposeAsync();
                                        }
                                    }
                                    else
                                    {
                                        result = "Invalid path";
                                    }
                                }
                                else
                                {
                                    result = "Internal server error";
                                }
                            }
                            else
                            {
                                result = "Invalid path";
                            }
                        }
                        else
                        {
                            result = "Invalid file name. Use only numbers, letters, '-' and '/'.";
                        }

                    }
                    else
                    {
                        result = "Invalid path";
                    }
                }
                else
                {
                    result = "Invalid file name";
                }
            }
            else
            {
                result = "Cannot read file content";
            }

            return result;
        }

        public Task<string?> Update(FtpModel? value)
        {
            throw new NotImplementedException();
        }

        private bool IsValidPath(string path_name) => path_name[0] == '/' ? path_name.All(c => char.IsLetter(c) || char.IsNumber(c) || c == '_' || c == '-' || c == ' ' || c == '/') : false;
        private bool IsValidFileName(string file_name) => file_name.All(c => char.IsLetter(c) || char.IsNumber(c) || c == '_' || c == '-' || c == ' ' || c == '.');
        private string? PathConverter(string? path) => OperatingSystem.IsWindows() == true ? path?.Replace('/', '\\') : path;
        private bool CheckIfDirectoryOnDisk(string? path) => IsValidDiskPath(path) == false ? false : true;
        private char PathSeparator() => OperatingSystem.IsWindows() == true ? '\\' : '/';
        private bool IsValidDiskPath(string? path) => Directory.Exists(path);


        private bool CreateUserRootDir(string? email)
        {
            char path_separator = PathSeparator();

            StringBuilder path_builder = new StringBuilder(Environment.CurrentDirectory);
            path_builder.Append(path_separator);
            path_builder.Append("FTP_Server");
            path_builder.Append(path_separator);
            path_builder.Append(email);

            string user_dir = path_builder.ToString();
            Console.WriteLine(user_dir);

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

        private string PathConverter(string? path, string? email)
        {
            char path_separator = PathSeparator();
            string? converted_path = PathConverter(path);

            StringBuilder path_builder = new StringBuilder(Environment.CurrentDirectory);
            path_builder.Append(path_separator);
            path_builder.Append("FTP_Server");
            path_builder.Append(path_separator);
            path_builder.Append(email);
            path_builder.Append(PathConverter(path));

            return path_builder.ToString();
        }
    }
}
