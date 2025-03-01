using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;
using Serilog;

namespace ThetaFTP.Shared.Controllers
{
    public class FtpDirectoryDatabaseController : CRUD_Interface_Payload<FtpDirectoryModel, Metadata,FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel>
    {
        public Task<PayloadModel?> Delete(FtpDirectoryModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (value != null)
            {
                if (value.email != null)
                {
                    if (value.path != null)
                    {
                        if (value.directory_name != null)
                        {
                            string converted_path = FileSystemFormatter.PathConverter(value?.path, value?.email);
                            string full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.directory_name);

                            if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                            {
                                if (FileSystemFormatter.IsValidUserDir(converted_path) == true)
                                {
                                    try
                                    {
                                        if (value != null)
                                            FileSystemFormatter.DeleteDirectory(full_path);
                                        payloadModel.result = "Directory deletion successful";
                                        payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                    }
                                    catch(Exception e)
                                    {
                                        Log.Error(e, "Directory FTP Controller delete error");
                                        payloadModel.result = "Internal server error";
                                    }
                                }
                                else
                                {
                                    payloadModel.result = "Invalid path";
                                }
                            }
                            else
                            {
                                payloadModel.result = "Invalid path";
                            }
                        }
                        else
                        {
                            payloadModel.result = "Invalid directory name";
                        }
                    }
                    else
                    {
                        payloadModel.result = "Invalid path";
                    }
                }
                else
                {
                    payloadModel.result = "Invalid email";
                }
            }
            else
            {
                payloadModel.result = "Internal server error";
            }

            return Task.FromResult<PayloadModel?>(payloadModel);
        }

        public Task<PayloadModel?> Get(FtpDirectoryModel? value)
        {
            throw new NotImplementedException();
        }

        public Task<PayloadModel?> GetInfo(Metadata? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

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
                                if (FileSystemFormatter.IsValidUserDir(converted_path) == true)
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

                                        payloadModel.payload = directories_info;
                                        payloadModel.result = "Directories retrieval successful";
                                        payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error(e, "Directory FTP Controller get info error");
                                        payloadModel.payload = "Internal server error";
                                    }
                                }
                                else
                                {
                                    payloadModel.payload = "Invalid path";
                                }
                            }
                            else
                            {
                                payloadModel.payload = "Invalid path";
                            }
                        }
                        else
                        {
                            payloadModel.payload = "Internal server error";
                        }
                    }
                    else
                    {
                        payloadModel.payload = "Invalid path";
                    }
                }
                else
                {
                    payloadModel.payload = "Invalid path";
                }
            }
            else
            {
                payloadModel.payload = "Internal server error";
            }

            return Task.FromResult<PayloadModel?>(payloadModel);
        }

        public Task<PayloadModel?> Insert(FtpDirectoryModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

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
                                        if (FileSystemFormatter.IsValidUserDir(converted_path) == true)
                                        {
                                            try
                                            {
                                                Directory.CreateDirectory(FileSystemFormatter.FullPathBuilder(converted_path, value?.directory_name));
                                                payloadModel.result = "Directory upload successful";
                                                payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                            }
                                            catch (Exception e)
                                            {
                                                Log.Error(e, "Directory FTP Controller upload error");
                                                payloadModel.result = "Internal server error";
                                            }
                                        }
                                        else
                                        {
                                            payloadModel.result = "Invalid path";
                                        }
                                    }
                                    else
                                    {
                                        payloadModel.result = "Invalid path";
                                    }
                                }
                                else
                                {
                                    payloadModel.result = "Internal server error";
                                }
                            }
                            else
                            {
                                payloadModel.result = "Invalid path";
                            }
                        }
                        else
                        {
                            payloadModel.result = "Invalid directory name. Use only numbers, letters, '_', '-', ' ', and '.'";
                        }
                    }
                    else
                    {
                        payloadModel.result = "Directory name more than 100 characters long";
                    }
                }
                else
                {
                    payloadModel.result = "Invalid path";
                }
            }
            else
            {
                payloadModel.result = "Invalid directory name";
            }

            return Task.FromResult<PayloadModel?>(payloadModel);
        }

        public Task<PayloadModel?> Rename(FtpDirectoryModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

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
                                string full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.directory_name);
                                string re_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.directory_new_name);

                                if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                                {
                                    if (FileSystemFormatter.IsValidUserDir(converted_path) == true)
                                    {
                                        if(FileSystemFormatter.IsValidUserDir(re_path) == true)
                                        {
                                            if (File.Exists(re_path) == false)
                                            {
                                                try
                                                {
                                                    if (value != null)
                                                        FileSystemFormatter.RenameDirectory(full_path, re_path);
                                                    payloadModel.result = "Directory rename successful";
                                                    payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.Error(e, "Directory FTP Controller upload error");
                                                    payloadModel.result = "Invalid directory name";
                                                }
                                            }
                                            else
                                            {
                                                payloadModel.result = "Invalid directory name";
                                            }
                                        }
                                        else
                                        {
                                            payloadModel.result = "Invalid path";
                                        }
                                    }
                                    else
                                    {
                                        payloadModel.result = "Invalid path";
                                    }
                                }
                                else
                                {
                                    payloadModel.result = "Invalid path";
                                }
                            }
                            else
                            {
                                payloadModel.result = "Invalid path";
                            }
                        }
                        else
                        {
                            payloadModel.result = "Invalid directory name";
                        }
                    }
                    else
                    {
                        payloadModel.result = "Invalid directory";
                    }
                }
                else
                {
                    payloadModel.result = "Internal server error";
                }
            }
            else
            {
                payloadModel.result = "Internal server error";
            }

            return Task.FromResult<PayloadModel?>(payloadModel);
        }

        public Task<PayloadModel?> Update(FtpDirectoryModel? value)
        {
            PayloadModel payloadModel = new PayloadModel();

            if (value?.directory_name != null)
            {
                if (value?.path != null)
                {
                    if (value?.new_path != null)
                    {
                        if (FileSystemFormatter.IsValidFileName(value.directory_name) == true)
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
                                                if (FileSystemFormatter.IsValidUserDir(converted_path) == true)
                                                {
                                                    if (FileSystemFormatter.IsValidUserDir(converted_new_path) == true)
                                                    {
                                                        try
                                                        {
                                                            string old_full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.directory_name);
                                                            string new_full_path = FileSystemFormatter.FullPathBuilder(converted_new_path, value?.directory_name);
                                                            FileSystemFormatter.MoveDirectory(old_full_path, new_full_path);
                                                            payloadModel.result = "Directory relocation successful";
                                                            payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Log.Error(e, "Directory FTP controller relocation error");

                                                            if (e.Message.Contains("already exists") == true)
                                                                payloadModel.result = "File already exist";
                                                            else
                                                                payloadModel.result = "Internal server error";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        payloadModel.result = "Invalid path";
                                                    }
                                                }
                                                else
                                                {
                                                    payloadModel.result = "Invalid path";
                                                }
                                            }
                                            else
                                            {
                                                payloadModel.result = "Invalid path";
                                            }
                                        }
                                        else
                                        {
                                            payloadModel.result = "Invalid path";
                                        }
                                    }
                                    else
                                    {
                                        payloadModel.result = "Internal server error";
                                    }
                                }
                                else
                                {
                                    payloadModel.result = "Invalid path";
                                }
                            }
                            else
                            {
                                payloadModel.result = "Invalid path";
                            }
                        }
                        else
                        {
                            payloadModel.result = "Invalid file name. Use only numbers, letters, '-', '/', '_', ' ', and '.'";
                        }
                    }
                    else
                    {
                        payloadModel.result = "Invalid path";
                    }
                }
                else
                {
                    payloadModel.result = "Invalid path";
                }
            }
            else
            {
                payloadModel.result = "Invalid file name";
            }

            return Task.FromResult<PayloadModel?>(payloadModel);
        }
    }
}
