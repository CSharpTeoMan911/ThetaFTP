using MySqlConnector;
using System.Text;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
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
                        if (value?.file_name.Length <= 100)
                        {
                            if (value?.size <= 65536000)
                            {
                                if (FileSystemFormatter.GetAvailableSpace() > value?.size)
                                {
                                    if (FileSystemFormatter.IsValidFileName(value.file_name) == true)
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
                                                        MySqlCommand insert_file_command = connection.CreateCommand();
                                                        try
                                                        {
                                                            try
                                                            {
                                                                string full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.file_name);
                                                                FileStream file_stream = File.OpenWrite(full_path);
                                                                try
                                                                {
                                                                    if (value != null)
                                                                    {
                                                                        bool file_upload_result = await StreamOperations.ReadAsync(value.fileStream, value.size, file_stream, 102400, 3);

                                                                        if(file_upload_result == true)
                                                                        {
                                                                            insert_file_command.CommandText = "INSERT INTO Files VALUES(@File_Name, @File_Path, @Email)";
                                                                            insert_file_command.Parameters.AddWithValue("File_Name", FileSystemFormatter.DatabaseKeyBuilder(value?.email, value?.file_name));
                                                                            insert_file_command.Parameters.AddWithValue("File_Path", value?.path);
                                                                            insert_file_command.Parameters.AddWithValue("Email", value?.email);

                                                                            await insert_file_command.ExecuteNonQueryAsync();
                                                                            result = "File upload successful";
                                                                        }
                                                                        else
                                                                        {
                                                                            if (File.Exists(full_path) == true)
                                                                                File.Delete(full_path);
                                                                        }

                                                                        result = "File upload successful";
                                                                    }
                                                                    else
                                                                    {
                                                                        result = "Internal server error";
                                                                    }
                                                                }
                                                                catch
                                                                {
                                                                    result = "File already exists";
                                                                }
                                                                finally
                                                                {
                                                                    await file_stream.DisposeAsync();
                                                                }
                                                            }
                                                            catch
                                                            {
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
                                        result = "Invalid file name. Use only numbers, letters, '-', '/', '_', ' ', and '.'";
                                    }
                                }
                                else
                                {
                                    result = "Insufficient space on disk";
                                }
                            }
                            else
                            {
                                result = "File size exceeds 500 MB";
                            }
                        }
                        else
                        {
                            result = "File name more than 100 characters long";
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


    }
}
