using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    public class FtpDatabaseController : CRUD_Interface<FtpModel, Metadata, FtpModel, FtpModel, FtpModel>
    {
        public async Task<string?> Delete(FtpModel? value)
        {
            string result = "Internal server error";

            if(value != null)
            {
                if (value.email != null)
                {
                    if (value.file_name != null)
                    {
                        if (value.path != null)
                        {
                            if (value.size <= 524288000)
                            {
                                MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();
                                try
                                {
                                    MySqlCommand get_file_metadata_command = connection.CreateCommand();
                                    try
                                    {

                                    }
                                    finally
                                    {

                                    }
                                }
                                finally
                                {
                                    await connection.DisposeAsync();
                                }
                            }
                            else
                            {
                                result = "File size exceeds 500 MB";
                            }
                        }
                        else
                        {
                            result = "Invalid path";
                        }
                    }
                    else
                    {
                        result = "Invalid file";
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

        public async Task<string?> Get(FtpModel? value)
        {
            string result = "Internal server error";

            if (value != null)
            {
                if (value.file_name != null)
                {
                    if (value.email != null)
                    {
                        if (value.path != null)
                        {
                            if (value.size <= 524288000)
                            {
                                MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();
                                try
                                {
                                    MySqlCommand get_file_metadata_command = connection.CreateCommand();
                                    try
                                    {
                                        string formatted_file_name = FileSystemFormatter.DatabaseKeyBuilder(value.email, value.file_name);
                                        get_file_metadata_command.CommandText = "SELECT File_Size, File_Path, Email FROM files WHERE File_Name = @File_Name";
                                        get_file_metadata_command.Parameters.AddWithValue("File_Name", formatted_file_name);

                                        DbDataReader get_file_metadata_command_reader = await get_file_metadata_command.ExecuteReaderAsync();
                                        try
                                        {
                                            if (await get_file_metadata_command_reader.ReadAsync() == true)
                                            {
                                                long file_size = (int)get_file_metadata_command_reader.GetValue(0);
                                                string file_path = get_file_metadata_command_reader.GetString(1);
                                                string email = get_file_metadata_command_reader.GetString(2);

                                                await get_file_metadata_command_reader.CloseAsync();

                                                if (file_path == value.path || file_size == value.size || email == value.email)
                                                {
                                                    string converted_path = FileSystemFormatter.PathConverter(value.path, value.email);

                                                    if (FileSystemFormatter.IsValidDiskPath(converted_path))
                                                    {
                                                        result = FileSystemFormatter.FullPathBuilder(converted_path, value.file_name);
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
                                        }
                                        finally
                                        {
                                            await get_file_metadata_command_reader.DisposeAsync();
                                        }

                                    }
                                    finally
                                    {
                                        await get_file_metadata_command.DisposeAsync();
                                    }

                                }
                                finally
                                {
                                    await connection.DisposeAsync();
                                }
                            }
                            else
                            {
                                result = "File size exceeds 500 MB";
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
                    result = "Invalid file name";
                }
            }
            else
            {
                result = "Internal server error";
            }

            return result;
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
                                    List<DirectoryItem> file_info = new List<DirectoryItem>();
                                    DirectoryInfo directoryInfo = new DirectoryInfo(converted_path);
                                    directoryInfo.GetFiles()?.ToList()?.ForEach(fileInfo => file_info.Add(new DirectoryItem()
                                    {
                                        name = fileInfo.Name,
                                        size = fileInfo.Length,
                                        isDirectory = false
                                    }));

                                    string? serialised_file_names = await JsonFormatter.JsonSerialiser(file_info);
                                    return serialised_file_names;
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
                            if (value?.size <= 524288000)
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
                                                                        bool file_upload_result = await StreamOperations.ReadAsync(value.fileStream, value.size, file_stream, 102400, 3, value.operation_cancellation);
                                                                        FileInfo uploaded_file = new FileInfo(full_path);
                                                                        string formatted_file_name = FileSystemFormatter.DatabaseKeyBuilder(value?.email, value?.file_name);

                                                                        if (file_upload_result == true)
                                                                        {
                                                                            insert_file_command.CommandText = "INSERT INTO files VALUES(@File_Name, @File_Size, @File_Path, @Email)";
                                                                            insert_file_command.Parameters.AddWithValue("File_Name", formatted_file_name);
                                                                            insert_file_command.Parameters.AddWithValue("File_Size", value?.size);
                                                                            insert_file_command.Parameters.AddWithValue("File_Path", value?.path);
                                                                            insert_file_command.Parameters.AddWithValue("Email", value?.email);

                                                                            await insert_file_command.ExecuteNonQueryAsync();

                                                                            if (value?.operation_cancellation.IsCancellationRequested == true)
                                                                            {
                                                                                if (File.Exists(full_path) == true)
                                                                                {
                                                                                    file_stream?.Dispose();
                                                                                    uploaded_file.Delete();
                                                                                }

                                                                                MySqlCommand delete_file_command = connection.CreateCommand();
                                                                                try
                                                                                {
                                                                                    delete_file_command.CommandText = "DELETE FROM files WHERE File_Name=@File_Name";
                                                                                    delete_file_command.Parameters.AddWithValue("File_Name", formatted_file_name);
                                                                                    await delete_file_command.ExecuteNonQueryAsync();
                                                                                }
                                                                                finally
                                                                                {
                                                                                    await delete_file_command.DisposeAsync();
                                                                                }

                                                                                result = "Operation cancelled";
                                                                            }
                                                                            else
                                                                            {
                                                                                result = "File upload successful";
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (File.Exists(full_path) == true)
                                                                            {
                                                                                file_stream?.Dispose();
                                                                                uploaded_file.Delete();
                                                                            }

                                                                            result = "Operation cancelled";
                                                                        }
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

        private enum OperationType
        {
            Download,
            Upload,
            Delete,
            Update
        }

        private async Task<string?> GetPendingFileTransferOperations(string? path, string? file, OperationType operationType)
        {
            string? result = "Internal server error";

            if (path != null)
            {
                if (file != null)
                {
                    MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();
                    try
                    {
                        MySqlCommand get_pending_file_transffer_operations = connection.CreateCommand();
                        try
                        {
                            get_pending_file_transffer_operations.CommandText = "SELECT Source_Path, Destination_Path FROM file_transfer_operations";
                            DbDataReader get_pending_file_transffer_operations_reader = await get_pending_file_transffer_operations.ExecuteReaderAsync();
                            try
                            {
                                if (await get_pending_file_transffer_operations_reader.ReadAsync() == true)
                                {
                                    string source_path = get_pending_file_transffer_operations_reader.GetString(0);
                                    string destination_path = get_pending_file_transffer_operations_reader.GetString(1);
                                }
                            }
                            finally
                            {
                                await get_pending_file_transffer_operations_reader.DisposeAsync();
                            }


                            switch (operationType)
                            {
                                case OperationType.Download:
                                    result = "File transfer in process";
                                    break;
                                case OperationType.Upload:
                                    break;
                                case OperationType.Delete:
                                    break;
                                case OperationType.Update:
                                    break;
                            }
                        }
                        finally
                        {
                            await get_pending_file_transffer_operations.DisposeAsync();
                        }
                    }
                    finally
                    {
                        await connection.DisposeAsync();
                    }
                }
                else
                {
                    result = "Invalid file";
                }
            }
            else
            {
                result = "Invalid path";
            }

            return result;
        }
    }
}
