﻿using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;
using Serilog;

namespace ThetaFTP.Shared.Controllers
{
    public class FtpDirectoryDatabaseController : CRUD_Interface<FtpDirectoryModel, Metadata,FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel>
    {
        public Task<string?> Delete(FtpDirectoryModel? value)
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
                            string full_path = FileSystemFormatter.FullPathBuilder(converted_path, value?.directory_name);

                            if (FileSystemFormatter.IsValidDiskPath(converted_path) == true)
                            {
                                if (FileSystemFormatter.IsValidUserDir(converted_path) == true)
                                {
                                    try
                                    {
                                        if (value != null)
                                            FileSystemFormatter.DeleteDirectory(full_path);
                                        result = "Directory deletion successful";
                                    }
                                    catch(Exception e)
                                    {
                                        Log.Error(e, "Directory FTP Controller delete error");
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

            return Task.FromResult<string?>(result);
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

                                        string? serialised_directory_names = await JsonFormatter.JsonSerialiser(directories_info);
                                        return serialised_directory_names;
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error(e, "Directory FTP Controller get info error");
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

        public Task<string?> Insert(FtpDirectoryModel? value)
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
                                        if (FileSystemFormatter.IsValidUserDir(converted_path) == true)
                                        {
                                            try
                                            {
                                                Directory.CreateDirectory(FileSystemFormatter.FullPathBuilder(converted_path, value?.directory_name));
                                                result = "Directory upload successful";
                                            }
                                            catch (Exception e)
                                            {
                                                Log.Error(e, "Directory FTP Controller upload error");
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

            return Task.FromResult<string?>(result);
        }

        public Task<string?> Rename(FtpDirectoryModel? value)
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
                                                    result = "Directory rename successful";
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.Error(e, "Directory FTP Controller upload error");
                                                    result = "Invalid directory name";
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

            return Task.FromResult<string?>(result);
        }

        public Task<string?> Update(FtpDirectoryModel? value)
        {
            string? result = "Internal server error";

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
                                                            result = "Directory relocation successful";
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Log.Error(e, "Directory FTP controller relocation error");

                                                            if (e.Message.Contains("already exists") == true)
                                                                result = "File already exist";
                                                            else
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
    }
}
