using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    public class FtpDirectoryDatabaseController : CRUD_Interface<FtpDirectoryModel, Metadata,FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel>
    {
        public async Task<string?> Delete(FtpDirectoryModel? value)
        {
            string result = "Internal server error";

            if (value != null)
            {
                if (value.email != null)
                {
                    if (value.path != null)
                    {
                        if (value.directory_name != null)
                        {
                            string converted_path = FileSystemFormatter.PathConverter(value?.path, value?.email);
                            string formatted_directory_name = FileSystemFormatter.DatabaseKeyBuilder(value?.email, value?.directory_name);
                            string full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.directory_name);

                            if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                            {
                                MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();
                                try
                                {

                                    MySqlCommand delete_file_metadata_command = connection.CreateCommand();
                                    try
                                    {
                                        delete_file_metadata_command.CommandText = "DELETE FROM directories WHERE Directory_Name = @Directory_Name";
                                        delete_file_metadata_command.Parameters.AddWithValue("Directory_Name", formatted_directory_name);
                                        await delete_file_metadata_command.ExecuteNonQueryAsync();

                                        if (value != null)
                                            FileSystemFormatter.DeleteDirectory(full_path);
                                        result = "Directory deletion successful";
                                    }
                                    finally
                                    {
                                        await delete_file_metadata_command.DisposeAsync();
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
                            result = "Invalid directory name";
                        }
                    }
                    else
                    {
                        result = "Invalid path";
                    }
                }
                else
                {
                    result = "Invalid email";
                }
            }
            else
            {
                result = "Internal server error";
            }

            return result;
        }

        public Task<string?> Get(FtpDirectoryModel? value)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> GetInfo(Metadata? value)
        {
            string? result = "Internal server error";

            if (value != null)
            {
                if (value?.path != null)
                {
                    if (FileSystemFormatter.IsValidPath(value.path) == true)
                    {
                        if (FileSystemFormatter.CreateUserRootDir(value.email) == true)
                        {
                            string converted_path = FileSystemFormatter.PathConverter(value.path, value.email);

                            if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                            {
                                try
                                {
                                    List<DirectoryItem> directories_info = new List<DirectoryItem>();
                                    DirectoryInfo directoryInfo = new DirectoryInfo(converted_path);
                                    directoryInfo.GetDirectories()?.ToList()?.ForEach(directoryInfo => directories_info.Add(new DirectoryItem()
                                    {
                                        name = directoryInfo.Name,
                                        isDirectory = true
                                    }));

                                    string? serialised_directory_names = await JsonFormatter.JsonSerialiser(directories_info);
                                    return serialised_directory_names;
                                }
                                catch
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
                    result = "Invalid path";
                }
            }
            else
            {
                result = "Internal server error";
            }

            return result;
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
                                if (FileSystemFormatter.CreateUserRootDir(value?.email) == true)
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
                                                create_directory_command.CommandText = "INSERT INTO directories VALUES(@Directory_Name, @Directory_Path, @Email)";
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

        public async Task<string?> Rename(FtpDirectoryModel? value)
        {
            string result = "Internal server error";

            if (value != null)
            {
                if (value.email != null)
                {
                    if (value.directory_name != null)
                    {
                        if (value.directory_new_name != null)
                        {
                            if (value.path != null)
                            {
                                string converted_path = FileSystemFormatter.PathConverter(value?.path, value?.email);
                                string formatted_file_name = FileSystemFormatter.DatabaseKeyBuilder(value?.email, value?.directory_name);
                                string formatted_new_file_name = FileSystemFormatter.DatabaseKeyBuilder(value?.email, value?.directory_new_name);
                                string full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.directory_name);
                                string re_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.directory_new_name);


                                Console.WriteLine($"converted_path: {full_path}");
                                bool operation_found = false;



                                if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                                {
                                    if (File.Exists(re_path) == false)
                                    {
                                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();
                                        try
                                        {

                                            MySqlCommand update_file_metadata_command = connection.CreateCommand();

                                            try
                                            {

                                                try
                                                {
                                                    if (value != null)
                                                        FileSystemFormatter.RenameDirectory(full_path, re_path);
                                                    result = "Directory rename successful";

                                                    update_file_metadata_command.CommandText = "UPDATE directories SET Directory_Name = @New_Directory_Name WHERE Directory_Name = @Directory_Name";
                                                    update_file_metadata_command.Parameters.AddWithValue("New_Directory_Name", formatted_new_file_name);
                                                    update_file_metadata_command.Parameters.AddWithValue("Directory_Name", formatted_file_name);
                                                    await update_file_metadata_command.ExecuteNonQueryAsync();
                                                }
                                                catch
                                                {
                                                    result = "Invalid directory name";
                                                }
                                            }
                                            finally
                                            {
                                                await update_file_metadata_command.DisposeAsync();
                                            }
                                        }
                                        finally
                                        {
                                            await connection.DisposeAsync();
                                        }
                                    }
                                    else
                                    {
                                        result = "Invalid directory name";
                                    }
                                }
                                else
                                {
                                    result = "Invalid path";
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
                    }
                    else
                    {
                        result = "Invalid directory";
                    }
                }
                else
                {
                    result = "Internal server error";
                }
            }
            else
            {
                result = "Internal server error";
            }

            return result;
        }

        public async Task<string?> Update(FtpDirectoryModel? value)
        {
            throw new NotImplementedException();
        }
    }
}
