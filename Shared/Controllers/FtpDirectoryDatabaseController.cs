using MySqlConnector;
using System.Text;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    public class FtpDirectoryDatabaseController : CRUD_Interface<FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel>
    {
        public Task<string?> Delete(FtpDirectoryModel? value)
        {
            throw new NotImplementedException();
        }

        public Task<string?> Get(FtpDirectoryModel? value)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> Insert(FtpDirectoryModel? value)
        {
            string? result = "Internal server error";

            if (value?.directory_name != null)
            {
                if (value?.path != null)
                {
                    if (value?.directory_name.Length <= 100)
                    {
                        if (FileSystemFormatter.IsValidDirectoryName(value.directory_name) == true)
                        {
                            if (FileSystemFormatter.IsValidPath(value.path) == true)
                            {
                                if (FileSystemFormatter.CreateUserRootDir(value?.email))
                                {
                                    string converted_path = FileSystemFormatter.PathConverter(value?.path, value?.email);

                                    if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                                    {

                                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();
                                        try
                                        {
                                            MySqlCommand create_directory_command = connection.CreateCommand();
                                            try
                                            {
                                                create_directory_command.CommandText = "INSERT INTO Directories VALUES(@Directory_Name, @Directory_Path, @Email)";
                                                create_directory_command.Parameters.AddWithValue("Directory_Name", FileSystemFormatter.DatabaseKeyBuilder(value?.email, value?.directory_name));
                                                create_directory_command.Parameters.AddWithValue("Directory_Path", value?.path);
                                                create_directory_command.Parameters.AddWithValue("Email", value?.email);
                                                await create_directory_command.ExecuteNonQueryAsync();

                                                Directory.CreateDirectory(FileSystemFormatter.FullPathBuilder(converted_path, value?.directory_name));

                                                result = "Directory upload successful";
                                            }
                                            catch
                                            {
                                                result = "Directory already exists";
                                            }
                                            finally
                                            {
                                                await create_directory_command.DisposeAsync();
                                            }
                                        }
                                        catch
                                        {
                                            result = "Internal server error";
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
                            result = "Invalid directory name. Use only numbers, letters, '_', '-', ' ', and '.'";
                        }
                    }
                    else
                    {
                        result = "Directory name more than 100 characters long";
                    }
                }
                else
                {
                    result = "Invalid path";
                }
            }
            else
            {
                result = "Invalid directory name";
            }

            return result;
        }

        public Task<string?> Update(FtpDirectoryModel? value)
        {
            throw new NotImplementedException();
        }
    }
}
