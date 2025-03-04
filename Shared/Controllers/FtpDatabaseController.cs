using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;
using Serilog;
using System.Text;

namespace ThetaFTP.Shared.Controllers
{
    public class FtpDatabaseController : CRUD_Interface_Payload<FtpModel, Metadata, FtpModel, FtpModel, FtpModel, FtpModel>
    {

        public Task<PayloadModel?> Delete(FtpModel? value)
        {
            PayloadModel serverPayload = new PayloadModel();

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
                                try
                                {
                                    string converted_path = FileSystemFormatter.PathConverter(value?.path, value?.email);
                                    string full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.file_name);

                                    if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                                    {
                                        if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                                        {
                                            if (value != null)
                                                FileSystemFormatter.DeleteFile(full_path);
                                            serverPayload.result = "File deletion successful";
                                            serverPayload.StatusCode = System.Net.HttpStatusCode.OK;
                                        }
                                        else
                                        {
                                            serverPayload.result = "Invalid path";
                                        }
                                    }
                                    else
                                    {
                                        serverPayload.result = "Invalid path";
                                    }
                                }
                                catch(Exception e)
                                {
                                    Log.Error(e, "File FTP Controller delete error");
                                    serverPayload.result = "Internal server error";
                                }
                                
                            }
                            else
                            {
                                serverPayload.result = "File size exceeds 500 MB";
                            }
                        }
                        else
                        {
                            serverPayload.result = "Invalid path";
                        }
                    }
                    else
                    {
                        serverPayload.result = "Invalid file";
                    }
                }
                else
                {
                    serverPayload.result = "Internal server error";
                }
            }
            else
            {
                serverPayload.result = "Internal server error";
            }

            return Task.FromResult<PayloadModel?>(serverPayload);
        }

        public Task<PayloadModel?> Get(FtpModel? value)
        {
            PayloadModel serverPayload = new PayloadModel();

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
                                    if (FileSystemFormatter.IsValidUserDir(converted_path))
                                    {
                                        serverPayload.result = "File path extraction successful";
                                        serverPayload.payload = FileSystemFormatter.FullPathBuilder(converted_path, value.file_name);
                                        serverPayload.StatusCode = System.Net.HttpStatusCode.OK;
                                    }
                                    else
                                    {
                                        serverPayload.result = "Invalid path";
                                    }
                                }
                                else
                                {
                                    serverPayload.result = "Invalid path";
                                }
                            }
                            else
                            {
                                serverPayload.result = "File size exceeds 500 MB";
                            }
                        }
                        else
                        {
                            serverPayload.result = "Invalid path";
                        }
                    }
                    else
                    {
                        serverPayload.result = "Internal server error";
                    }
                }
                else
                {
                    serverPayload.result = "Invalid file name";
                }
            }
            else
            {
                serverPayload.result = "Internal server error";
            }

            return Task.FromResult<PayloadModel?>(serverPayload);
        }

        public Task<PayloadModel?> GetInfo(Metadata? value)
        {
            PayloadModel serverPayload = new PayloadModel();

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
                                if (FileSystemFormatter.IsValidUserDir(converted_path))
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

                                        serverPayload.StatusCode = System.Net.HttpStatusCode.OK;
                                        serverPayload.result = "Files retrieval successful";
                                        serverPayload.payload = file_info;
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error(e, "File FTP controller get info error");
                                        serverPayload.result = "Internal server error";
                                    }
                                }
                                else
                                {
                                    serverPayload.result = "Invalid path";
                                }
                            }
                            else
                            {
                                serverPayload.result = "Invalid path";
                            }
                        }
                        else
                        {
                            serverPayload.result = "Internal server error";
                        }
                    }
                    else
                    {
                        serverPayload.result = "Invalid path";
                    }
                }
                else
                {
                    serverPayload.result = "Invalid path";
                }
            }
            else
            {
                serverPayload.result = "Internal server error";
            }

            return Task.FromResult<PayloadModel?>(serverPayload);
        }

        public async Task<PayloadModel?> Insert(FtpModel? value)
        {
            PayloadModel serverPayload = new PayloadModel();

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
                                                    if (FileSystemFormatter.IsValidUserDir(converted_path))
                                                    {
                                                        if (value != null)
                                                        {
                                                            bool exists = false;
                                                            string full_path = FileSystemFormatter.GenerateValidPath(converted_path, value.file_name, out exists);

                                                            if (exists == false)
                                                            {
                                                                try
                                                                {
                                                                    using (FileStream file_stream = File.OpenWrite(full_path))
                                                                    {
                                                                        bool file_upload_result = false;

                                                                        if (Shared.configurations?.use_file_encryption == false)
                                                                        {
                                                                            file_upload_result = await StreamOperations.ReadAsync(value.fileStream, value.size, file_stream, 102400, 3, value.operation_cancellation);
                                                                        }
                                                                        else
                                                                        {
                                                                            AesFileEncryption? fileEncryption = Shared.GetAes();
                                                                            if (fileEncryption != null)
                                                                                file_upload_result = await fileEncryption.EncryptFile(value.fileStream, value.size, file_stream, 102400, 3, value.operation_cancellation);
                                                                        }

                                                                        FileInfo uploaded_file = new FileInfo(full_path);

                                                                        if (file_upload_result == true)
                                                                        {
                                                                            if (value?.operation_cancellation.IsCancellationRequested == true)
                                                                            {
                                                                                if (File.Exists(full_path) == true)
                                                                                {
                                                                                    uploaded_file.Delete();
                                                                                }

                                                                                serverPayload.result = "Operation cancelled";
                                                                                serverPayload.StatusCode = System.Net.HttpStatusCode.OK;
                                                                            }
                                                                            else
                                                                            {
                                                                                serverPayload.result = "File upload successful";
                                                                                serverPayload.StatusCode = System.Net.HttpStatusCode.OK;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (File.Exists(full_path) == true)
                                                                            {
                                                                                uploaded_file.Delete();
                                                                            }

                                                                            serverPayload.result = "Operation cancelled";
                                                                            serverPayload.StatusCode = System.Net.HttpStatusCode.OK;
                                                                        }
                                                                    }

                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Log.Error(e, "File FTP controller upload error");
                                                                    serverPayload.result = "Internal server error";
                                                                }
                                                            }
                                                            else
                                                            {
                                                                serverPayload.result = "Internal server error";
                                                            }

                                                        }
                                                        else
                                                        {
                                                            serverPayload.result = "Internal server error";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        serverPayload.result = "Invalid path";
                                                    }
                                                }
                                                else
                                                {
                                                    serverPayload.result = "Invalid path";
                                                }
                                            }
                                            else
                                            {
                                                serverPayload.result = "Internal server error";
                                            }
                                        }
                                        else
                                        {
                                            serverPayload.result = "Invalid path";
                                        }
                                    }
                                    else
                                    {
                                        serverPayload.result = "Invalid file name. Use only numbers, letters, '-', '/', '_', ' ', and '.'";
                                    }
                                }
                                else
                                {
                                    serverPayload.result = "Insufficient space on disk";
                                }
                            }
                            else
                            {
                                serverPayload.result = "File size exceeds 500 MB";
                            }
                        }
                        else
                        {
                            serverPayload.result = "File name more than 100 characters long";
                        }
                    }
                    else
                    {
                        serverPayload.result = "Invalid path";
                    }
                }
                else
                {
                    serverPayload.result = "Invalid file name";
                }
            }
            else
            {
                serverPayload.result = "Cannot read file content";
            }

            return serverPayload;
        }

        public Task<PayloadModel?> Update(FtpModel? value)
        {
            PayloadModel serverPayload = new PayloadModel();

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
                                            if (FileSystemFormatter.IsValidUserDir(converted_path))
                                            {
                                                if (FileSystemFormatter.IsValidUserDir(converted_new_path))
                                                {
                                                    try
                                                    {
                                                        string old_full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.file_name);
                                                        string new_full_path = FileSystemFormatter.FullPathBuilder(converted_new_path, value?.file_name);

                                                        FileSystemFormatter.MoveFile(old_full_path, new_full_path);
                                                        serverPayload.result = "File relocation successful";
                                                        serverPayload.StatusCode = System.Net.HttpStatusCode.OK;
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Log.Error(e, "File FTP controller relocation error");
                                                        
                                                        if(e.Message.Contains("Cannot create a file when that file already exists") == true)
                                                            serverPayload.result = "File already exist";
                                                        else
                                                            serverPayload.result = "Internal server error";
                                                    }
                                                }
                                                else
                                                {
                                                    serverPayload.result = "Invalid path";
                                                }
                                            }
                                            else
                                            {
                                                serverPayload.result = "Invalid path";
                                            }
                                        }
                                        else
                                        {
                                            serverPayload.result = "Invalid path";
                                        }
                                    }
                                    else
                                    {
                                        serverPayload.result = "Internal server error";
                                    }
                                }
                                else
                                {
                                    serverPayload.result = "Invalid path";
                                }
                            }
                            else
                            {
                                serverPayload.result = "Invalid path";
                            }
                        }
                        else
                        {
                            serverPayload.result = "Invalid file name. Use only numbers, letters, '-', '/', '_', ' ', and '.'";
                        }
                    }
                    else
                    {
                        serverPayload.result = "Invalid path";
                    }
                }
                else
                {
                    serverPayload.result = "Invalid path";
                }
            }
            else
            {
                serverPayload.result = "Invalid file name";
            }

            return Task.FromResult<PayloadModel?>(serverPayload);
        }

        public Task<PayloadModel?> Rename(FtpModel? value)
        {
            PayloadModel serverPayload = new PayloadModel();

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

                                if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                                {
                                    if (FileSystemFormatter.IsValidUserDir(converted_path))
                                    {
                                        if (File.Exists(re_path) == false)
                                        {
                                            try
                                            {
                                                if (value != null)
                                                    FileSystemFormatter.RenameFile(full_path, re_path);
                                                serverPayload.result = "File rename successful";
                                                serverPayload.StatusCode = System.Net.HttpStatusCode.OK;
                                            }
                                            catch (Exception e)
                                            {
                                                Log.Error(e, "File FTP controller relocation error");
                                                serverPayload.result = "Invalid file name";
                                            }
                                        }
                                        else
                                        {
                                            serverPayload.result = "Invalid file name";
                                        }
                                    }
                                    else
                                    {
                                        serverPayload.result = "Invalid path";
                                    }
                                }
                                else
                                {
                                    serverPayload.result = "Invalid path";
                                }
                            }
                            else
                            {
                                serverPayload.result = "Invalid path";
                            }
                        }
                        else
                        {
                            serverPayload.result = "Invalid file name";
                        }
                    }
                    else
                    {
                        serverPayload.result = "Invalid file";
                    }
                }
                else
                {
                    serverPayload.result = "Internal server error";
                }
            }
            else
            {
                serverPayload.result = "Internal server error";
            }

            return Task.FromResult<PayloadModel?>(serverPayload);
        }
    }
}
