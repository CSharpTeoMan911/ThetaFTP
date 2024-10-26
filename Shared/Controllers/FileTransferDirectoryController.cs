using Microsoft.AspNetCore.Mvc;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    [Route("/directories")]
    [ApiController]
    public class FileTransferDirectoryController : Controller, CRUD_Api_Interface<DirectoryOperationMetadata, string, DirectoryOperationMetadata, string, DirectoryOperationMetadata, string, DirectoryOperationMetadata, string>
    {
        [HttpDelete("delete")]
        public async Task<ActionResult?> Delete([FromQuery]DirectoryOperationMetadata? query, [FromBody]string? body)
        {
            string? result = "Internal server error";

            string payload = String.Empty;

            string? log_in_key_validation_result = "Internal server error";

            if (Shared.config != null)
                if (!Shared.config.use_firebase)
                    log_in_key_validation_result = await Shared.database_validation.ValidateLogInSessionKey(query?.key);
                else
                    log_in_key_validation_result = await Shared.firebase_database_validation.ValidateLogInSessionKey(query?.key);

            if (log_in_key_validation_result != "Internal server error")
            {
                if (log_in_key_validation_result != "Invalid log in session key")
                {
                    if (log_in_key_validation_result != "Log in session key expired")
                    {
                        if (log_in_key_validation_result != "Log in session not approved")
                        {
                            if (query != null)
                            {
                                FtpDirectoryModel ftpModel = new FtpDirectoryModel()
                                {
                                    email = log_in_key_validation_result,
                                    directory_name = query?.directory_name,
                                    path = query?.path
                                };

                                result = await Shared.database_directory_ftp.Delete(ftpModel);

                                return Ok(result);
                            }
                            else
                            {
                                return Ok(result);
                            }
                        }
                        else
                        {
                            return Ok(log_in_key_validation_result);
                        }
                    }
                    else
                    {
                        return Ok(log_in_key_validation_result);
                    }
                }
                else
                {
                    return Ok(log_in_key_validation_result);
                }
            }
            else
            {
                return Ok(log_in_key_validation_result);
            }
        }

        [HttpGet("get")]
        public Task<ActionResult?> Get([FromQuery] DirectoryOperationMetadata? query, string? body)
        {
            throw new NotImplementedException();
        }

        [HttpGet("get-directories")]
        public async Task<ActionResult?> GetInfo([FromQuery] Metadata? query)
        {
            string? result = "Internal server error";

            string payload = String.Empty;

            string? log_in_key_validation_result = "Internal server error";

            if (Shared.config != null)
                if (!Shared.config.use_firebase)
                    log_in_key_validation_result = await Shared.database_validation.ValidateLogInSessionKey(query?.key);
                else
                    log_in_key_validation_result = await Shared.firebase_database_validation.ValidateLogInSessionKey(query?.key);

            if (log_in_key_validation_result != "Internal server error")
            {
                if (log_in_key_validation_result != "Invalid log in session key")
                {
                    if (log_in_key_validation_result != "Log in session key expired")
                    {
                        if (log_in_key_validation_result != "Log in session not approved")
                        {
                            if (query != null)
                            {
                                query.email = log_in_key_validation_result;

                                result = await Shared.database_directory_ftp.GetInfo(query);

                                return Ok(result);
                            }
                            else
                            {
                                return Ok(result);
                            }
                        }
                        else
                        {
                            return Ok(log_in_key_validation_result);
                        }
                    }
                    else
                    {
                        return Ok(log_in_key_validation_result);
                    }
                }
                else
                {
                    return Ok(log_in_key_validation_result);
                }
            }
            else
            {
                return Ok(log_in_key_validation_result);
            }
        }

        [HttpPost("insert")]
        public async Task<ActionResult?> Insert([FromQuery] DirectoryOperationMetadata? query, [FromBody] string? body)
        {
            string? result = "Internal server error";

            string payload = String.Empty;

            string? log_in_key_validation_result = "Internal server error";

            if (Shared.config != null)
                if (!Shared.config.use_firebase)
                    log_in_key_validation_result = await Shared.database_validation.ValidateLogInSessionKey(query?.key);
                else
                    log_in_key_validation_result = await Shared.firebase_database_validation.ValidateLogInSessionKey(query?.key);

            if (log_in_key_validation_result != "Internal server error")
            {
                if (log_in_key_validation_result != "Invalid log in session key")
                {
                    if (log_in_key_validation_result != "Log in session key expired")
                    {
                        if (log_in_key_validation_result != "Log in session not approved")
                        {
                            FtpDirectoryModel directoryModel = new FtpDirectoryModel()
                            {
                                email = log_in_key_validation_result,
                                directory_name = query?.directory_name,
                                path = query?.path,
                            };

                            result = await Shared.database_directory_ftp.Insert(directoryModel);

                            return Ok(result);
                        }
                        else
                        {
                            return Ok(log_in_key_validation_result);
                        }
                    }
                    else
                    {
                        return Ok(log_in_key_validation_result);
                    }
                }
                else
                {
                    return Ok(log_in_key_validation_result);
                }
            }
            else
            {
                return Ok(log_in_key_validation_result);
            }
        }

        [HttpPut("relocate")]
        public Task<ActionResult?> Update(DirectoryOperationMetadata? query, string? body)
        {
            throw new NotImplementedException();
        }

        [HttpPut("rename")]
        public async Task<ActionResult?> UpdateName([FromQuery] DirectoryOperationMetadata? query, string? body)
        {
            string? result = "Internal server error";

            string payload = String.Empty;

            string? log_in_key_validation_result = "Internal server error";

            if (Shared.config != null)
                if (!Shared.config.use_firebase)
                    log_in_key_validation_result = await Shared.database_validation.ValidateLogInSessionKey(query?.key);
                else
                    log_in_key_validation_result = await Shared.firebase_database_validation.ValidateLogInSessionKey(query?.key);

            if (query != null)
            {
                if (log_in_key_validation_result != "Internal server error")
                {
                    if (log_in_key_validation_result != "Invalid log in session key")
                    {
                        if (log_in_key_validation_result != "Log in session key expired")
                        {
                            if (log_in_key_validation_result != "Log in session not approved")
                            {
                                FtpDirectoryModel directoryModel = new FtpDirectoryModel()
                                {
                                    email = log_in_key_validation_result,
                                    directory_name = query?.directory_name,
                                    directory_new_name = query?.new_directory_name,
                                    path = query?.path,
                                };

                                result = await Shared.database_directory_ftp.Rename(directoryModel);

                                return Ok(result);
                            }
                            else
                            {
                                return Ok(log_in_key_validation_result);
                            }
                        }
                        else
                        {
                            return Ok(log_in_key_validation_result);
                        }
                    }
                    else
                    {
                        return Ok(log_in_key_validation_result);
                    }
                }
                else
                {
                    return Ok(log_in_key_validation_result);
                }
            }
            else
            {
                return Ok(result);
            }
        }
    }
}
