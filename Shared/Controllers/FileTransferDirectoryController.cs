﻿using Microsoft.AspNetCore.Mvc;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    [Route("/directories")]
    [ApiController]
    public class FileTransferDirectoryController : Controller, CRUD_Api_Interface<DirectoryOperationMetadata, string, DirectoryOperationMetadata, string, DirectoryOperationMetadata, string, DirectoryOperationMetadata, string>
    {
        public Task<ActionResult?> Delete(DirectoryOperationMetadata? query, string? body)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult?> Get(DirectoryOperationMetadata? query, string? body)
        {
            throw new NotImplementedException();
        }

        [HttpPost("insert")]
        public async Task<ActionResult?> Insert([FromQuery] DirectoryOperationMetadata? query, [FromBody] string? body)
        {
            string? result = "Internal server error";

            string payload = String.Empty;

            string? log_in_key_validation_result = await Shared.database_validation.ValidateLogInSessionKey(query?.key);

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

                            if (result == "Directory upload successful")
                            {
                                return Ok(result);
                            }
                            else
                            {
                                return BadRequest(result);
                            }
                        }
                        else
                        {
                            return BadRequest(log_in_key_validation_result);
                        }
                    }
                    else
                    {
                        return BadRequest(log_in_key_validation_result);
                    }
                }
                else
                {
                    return BadRequest(log_in_key_validation_result);
                }
            }
            else
            {
                return BadRequest(log_in_key_validation_result);
            }
        }

        public Task<ActionResult?> Update(DirectoryOperationMetadata? query, string? body)
        {
            throw new NotImplementedException();
        }
    }
}