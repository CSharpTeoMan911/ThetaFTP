using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    public class FtpDatabaseController : CRUD_Interface<FtpModel, Metadata, FtpModel, FtpModel, FtpModel, FtpModel>
    {

        public Task<string?> Delete(FtpModel? value)
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

                                string converted_path = FileSystemFormatter.PathConverter(value?.path, value?.email);
                                string full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.file_name);
                                //bool operation_found = false;

                                
                                if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                                {
                                    if (value != null)
                                        FileSystemFormatter.DeleteFile(full_path);
                                    result = "File deletion successful";
                                }
                                else
                                {
                                    result = "Invalid path";
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

            return Task.FromResult<string?>(result);
        }

        public Task<string?> Get(FtpModel? value)
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

            return Task.FromResult<string?>(result);
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
                                                    string full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.file_name);

                                                    if(value != null)
                                                    {
                                                        FileStream file_stream = File.OpenWrite(full_path);
                                                        bool file_upload_result = await StreamOperations.ReadAsync(value.fileStream, value.size, file_stream, 102400, 3, value.operation_cancellation);

                                                        FileInfo uploaded_file = new FileInfo(full_path);

                                                        if (file_upload_result == true)
                                                        {
                                                            if (value?.operation_cancellation.IsCancellationRequested == true)
                                                            {
                                                                if (File.Exists(full_path) == true)
                                                                {
                                                                    file_stream?.Dispose();
                                                                    uploaded_file.Delete();
                                                                }

                                                                result = "Operation cancelled";
                                                            }
                                                            else
                                                            {
                                                                file_stream?.Dispose();
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
            string? result = "Internal server error";


            if (value?.file_name != null)
            {
                if (value?.path != null)
                {
                    if (value?.new_path != null)
                    {
                        if (FileSystemFormatter.IsValidFileName(value.file_name) == true)
                        {
                            if (FileSystemFormatter.IsValidPath(value.path) == true)
                            {
                                if (FileSystemFormatter.IsValidPath(value.new_path) == true)
                                {
                                    if (FileSystemFormatter.CreateUserRootDir(value?.email))
                                    {
                                        string converted_path = FileSystemFormatter.PathConverter(value?.path, value?.email);
                                        string converted_new_path = FileSystemFormatter.PathConverter(value?.new_path, value?.email);

                                        if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                                        {
                                            if (FileSystemFormatter.IsValidDiskPath(converted_new_path) == true)
                                            {
                                                string old_full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.file_name);
                                                string new_full_path = FileSystemFormatter.FullPathBuilder(converted_new_path, value?.file_name);

                                                FileSystemFormatter.MoveFile(old_full_path, new_full_path);
                                                result = "File relocation successful";
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
                            result = "Invalid file name. Use only numbers, letters, '-', '/', '_', ' ', and '.'";
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
                result = "Invalid file name";
            }

            return Task.FromResult<string?>(result);
        }

        public Task<string?> Rename(FtpModel? value)
        {
            string? result = "Internal server error";

            if (value != null)
            {
                if (value.email != null)
                {
                    if (value.file_name != null)
                    {
                        if (value.new_name != null)
                        {
                            if (value.path != null)
                            {
                                string converted_path = FileSystemFormatter.PathConverter(value?.path, value?.email);
                                string full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.file_name);
                                string re_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.new_name);

                                //bool operation_found = false;



                                if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                                {
                                    if (File.Exists(re_path) == false)
                                    {
                                        try
                                        {
                                            if (value != null)
                                                FileSystemFormatter.RenameFile(full_path, re_path);
                                            result = "File rename successful";
                                        }
                                        catch
                                        {
                                            result = "Invalid file name";
                                        }
                                    }
                                    else
                                    {
                                        result = "Invalid file name";
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
                            result = "Invalid file name";
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

            return Task.FromResult<string?>(result);
        }
    }
}
