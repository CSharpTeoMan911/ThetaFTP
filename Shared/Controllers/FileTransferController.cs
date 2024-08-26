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

        [HttpGet("get-files")]
        public async Task<ActionResult?> GetFiles([FromQuery] Metadata? query)
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
                            if (query != null)
                            {
                                query.email = log_in_key_validation_result;
                                result = await Shared.database_ftp.GetInfo(query);

                                if (result != "Invalid path" && result != "Internal server error")
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


        [HttpPost("insert")]
        public async Task<ActionResult?> Insert([FromQuery] FileOperationMetadata? query, [FromBody] Stream? body)
        {
            string? result = "Internal server error";

            string payload = String.Empty;

            string? log_in_key_validation_result = await Shared.database_validation.ValidateLogInSessionKey(query?.key);

            if (body != null)
            {
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
                                    long file_size = 0;
                                    if (query != null)
                                        file_size = query.file_length;

                                    FtpModel ftpModel = new FtpModel()
                                    {
                                        email = log_in_key_validation_result,
                                        file_name = query?.file_name,
                                        path = query?.path,
                                        fileStream = body,
                                        operation_cancellation = HttpContext.RequestAborted,
                                        size = file_size
                                    };


                                    result = await Shared.database_ftp.Insert(ftpModel);

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
                        return BadRequest(log_in_key_validation_result);
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
