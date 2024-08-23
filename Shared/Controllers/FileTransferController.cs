using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Text;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    [Route("files")]
    [ApiController]
    public class FileTransferController : Controller, CRUD_Api_Interface<FileOperationMetadata, Stream, FileOperationMetadata, Stream, FileOperationMetadata, Stream, FileOperationMetadata, Stream>
    {
        [HttpDelete("delete")]
        public Task<ActionResult?> Delete([FromQuery] FileOperationMetadata? query, [FromBody] Stream? body)
        {
            throw new NotImplementedException();
        }

        [HttpGet("get")]
        public Task<ActionResult?> Get([FromQuery] FileOperationMetadata? query, [FromBody] Stream? body)
        {
            throw new NotImplementedException();
        }

        [HttpPost("insert")]
        public async Task<ActionResult?> Insert([FromQuery] FileOperationMetadata? query, [FromBody] Stream? body)
        {
            string? result = "Internal server error";

            string payload = String.Empty;

            string? log_in_key_validation_result = await Shared.database_validation.ValidateLogInSessionKey(query?.key);

            
            if (log_in_key_validation_result != "Internal server error")
            {
                if (log_in_key_validation_result != "Invalid code")
                {
                    if (log_in_key_validation_result != "Invalid log in session key")
                    {
                        if (log_in_key_validation_result != "Log in session key expired")
                        {
                            if (log_in_key_validation_result != "Log in session not approved")
                            {
                                FtpModel ftpModel = new FtpModel()
                                {
                                    email = log_in_key_validation_result,
                                    file_name = query?.file_name,
                                    path = query?.path,
                                    fileStream = body
                                };

                                result = await Shared.database_ftp.Insert(ftpModel);

                                if (result == "Internal server error")
                                {
                                    return BadRequest(result);
                                }
                                else if(result == "File already exists")
                                {
                                    return BadRequest(result);
                                }
                                else if (result == "Cannot read file content")
                                {
                                    return BadRequest(result);
                                }
                                else if (result == "Invalid file name")
                                {
                                    return BadRequest(result);
                                }
                                else if (result == "Invalid path")
                                {
                                    return BadRequest(result);
                                }
                                else if (result == "Insufficient space on disk")
                                {
                                    return BadRequest(result);
                                }
                                else if (result == "Invalid file name. Use only numbers, letters, '-' and '/'.")
                                {
                                    return BadRequest(result);
                                }
                                else if (result == "File name more than 100 characters long")
                                {
                                    return BadRequest(result);
                                }
                                else if (result == "File size exceeds 500 MB")
                                {
                                    return BadRequest(result);
                                }
                                else
                                {
                                    return Ok(result);
                                }
                            }
                            else
                            {
                                return BadRequest(result);
                            }
                        }
                        else
                        {
                            return BadRequest(result);
                        }
                    }
                    else
                    {
                        return BadRequest(result);
                    }
                }
                else
                {
                    return BadRequest(result);
                }
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("update")]
        public Task<ActionResult?> Update([FromQuery] FileOperationMetadata? query, [FromBody] Stream? body)
        {
            throw new NotImplementedException();
        }
    }
}
