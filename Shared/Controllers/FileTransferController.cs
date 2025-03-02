using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Text;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    [Route("files")]
    [ApiController]
    public class FileTransferController : Controller, CRUD_Api_Interface<FileOperationMetadata, Stream, Metadata, string, FileOperationMetadata, Stream, FileOperationMetadata, Stream>
    {
        [HttpDelete("delete")]
        public async Task<ActionResult?> Delete([FromQuery] FileOperationMetadata? query, [FromBody] Stream? body)
        {
            PayloadModel? serverPayload = new PayloadModel();

            try
            {
                if (Shared.configurations != null && query != null)
                {
                    if (!Shared.configurations.use_firebase)
                        serverPayload = await Shared.database_validation.ValidateLogInSessionKey(query?.key);
                    else
                        serverPayload = await Shared.firebase_database_validation.ValidateLogInSessionKey(query?.key);

                    if (serverPayload?.StatusCode == System.Net.HttpStatusCode.OK && serverPayload?.payload?.GetType() == typeof(string))
                    {
                        long file_size = 0;
                        if (query != null)
                            file_size = query.file_length;

                        FtpModel ftpModel = new FtpModel()
                        {
                            email = (string)serverPayload.payload,
                            file_name = query?.file_name,
                            path = query?.path,
                            operation_cancellation = HttpContext.RequestAborted,
                            size = file_size
                        };

                        serverPayload = await Shared.database_ftp.Delete(ftpModel);
                    }
                }

                if (serverPayload?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok(serverPayload);
                }
                else
                {
                    return StatusCode(500, serverPayload);
                }
            }
            catch
            {
                return StatusCode(500, serverPayload);
            }
        }

        [HttpGet("get")]
        public async Task<Stream?> GetFile([FromQuery] FileOperationMetadata? query, string? body)
        {
            PayloadModel? payloadModel = new PayloadModel();
            Stream? stream = null;

            try
            {
                if (Shared.configurations != null)
                {
                    if (!Shared.configurations.use_firebase)
                        payloadModel = await Shared.database_validation.ValidateLogInSessionKey(query?.key);
                    else
                        payloadModel = await Shared.firebase_database_validation.ValidateLogInSessionKey(query?.key);

                    if (query != null && payloadModel?.StatusCode == System.Net.HttpStatusCode.OK && payloadModel?.payload?.GetType() == typeof(string))
                    {
                        FtpModel ftpModel = new FtpModel()
                        {
                            email = (string)payloadModel.payload,
                            file_name = query.file_name,
                            path = query.path,
                            size = query.file_length,
                            operation_cancellation = HttpContext.RequestAborted
                        };

                        payloadModel = await Shared.database_ftp.Get(ftpModel);

                        if (payloadModel?.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            if (payloadModel?.result == "File path extraction successful")
                            {
                                if (payloadModel?.payload?.GetType() == typeof(string))
                                    stream = System.IO.File.OpenRead((string)payloadModel.payload);
                            }
                        }
                    }
                }
            }
            catch { }

            return stream;
        }

        [HttpGet("get-files")]
        public async Task<ActionResult?> Get([FromQuery] Metadata? query, string? body)
        {
            PayloadModel? serverPayload = new PayloadModel();

            try
            {

                if (Shared.configurations != null)
                {
                    if (!Shared.configurations.use_firebase)
                        serverPayload = await Shared.database_validation.ValidateLogInSessionKey(query?.key);
                    else
                        serverPayload = await Shared.firebase_database_validation.ValidateLogInSessionKey(query?.key);

                    if (query != null && serverPayload?.StatusCode == System.Net.HttpStatusCode.OK && serverPayload?.payload?.GetType() == typeof(string))
                    {
                        query.email = (string)serverPayload.payload;

                        serverPayload = await Shared.database_ftp.GetInfo(query);
                    }
                }


                if (serverPayload?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok(serverPayload);
                }
                else
                {
                    return StatusCode(500, serverPayload);
                }
            }
            catch
            {
                return StatusCode(500, serverPayload);
            }
        }


        [HttpPost("insert")]
        public async Task<ActionResult?> Insert([FromQuery] FileOperationMetadata? query, [FromBody] Stream? body)
        {
            PayloadModel? serverPayload = new PayloadModel();

            try
            {
                if (Shared.configurations != null)
                {
                    if (!Shared.configurations.use_firebase)
                        serverPayload = await Shared.database_validation.ValidateLogInSessionKey(query?.key);
                    else
                        serverPayload = await Shared.firebase_database_validation.ValidateLogInSessionKey(query?.key);

                    if (query != null && serverPayload?.StatusCode == System.Net.HttpStatusCode.OK && serverPayload?.payload?.GetType() == typeof(string))
                    {
                        long file_size = 0;
                        if (query != null)
                            file_size = query.file_length;

                        FtpModel ftpModel = new FtpModel()
                        {
                            email = (string)serverPayload.payload,
                            file_name = query?.file_name,
                            path = query?.path,
                            fileStream = body,
                            operation_cancellation = HttpContext.RequestAborted,
                            size = file_size
                        };

                        serverPayload = await Shared.database_ftp.Insert(ftpModel);
                    }
                }

                if (serverPayload?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok(serverPayload);
                }
                else
                {
                    return StatusCode(500, serverPayload);
                }
            }
            catch
            {
                return StatusCode(500, serverPayload);
            }
        }

        [HttpPut("relocate")]
        public async Task<ActionResult?> Update([FromQuery] FileOperationMetadata? query, [FromBody] Stream? body)
        {
            PayloadModel? serverPayload = new PayloadModel();

            try
            {
                if (Shared.configurations != null)
                {
                    if (!Shared.configurations.use_firebase)
                        serverPayload = await Shared.database_validation.ValidateLogInSessionKey(query?.key);
                    else
                        serverPayload = await Shared.firebase_database_validation.ValidateLogInSessionKey(query?.key);

                    if (query != null && serverPayload?.StatusCode == System.Net.HttpStatusCode.OK && serverPayload?.payload?.GetType() == typeof(string))
                    {
                        FtpModel ftpModel = new FtpModel()
                        {
                            email = (string)serverPayload.payload,
                            file_name = query?.file_name,
                            path = query?.path,
                            new_path = query?.new_path,
                        };

                        serverPayload = await Shared.database_ftp.Update(ftpModel);
                    }
                }

                if (serverPayload?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok(serverPayload);
                }
                else
                {
                    return StatusCode(500, serverPayload);
                }
            }
            catch
            {
                return StatusCode(500, serverPayload);
            }
        }

        [HttpPut("rename")]
        public async Task<ActionResult?> UpdateName([FromQuery] RenameModel? query, [FromBody] Stream? body)
        {
            PayloadModel? serverPayload = new PayloadModel();

            try
            {
                if (Shared.configurations != null)
                {
                    if (!Shared.configurations.use_firebase)
                        serverPayload = await Shared.database_validation.ValidateLogInSessionKey(query?.key);
                    else
                        serverPayload = await Shared.firebase_database_validation.ValidateLogInSessionKey(query?.key);

                    if (query != null && serverPayload?.StatusCode == System.Net.HttpStatusCode.OK && serverPayload?.payload?.GetType() == typeof(string))
                    {
                        FtpModel ftpModel = new FtpModel()
                        {
                            email = (string)serverPayload.payload,
                            file_name = query?.file_name,
                            new_name = query?.new_name,
                            path = query?.path,
                            operation_cancellation = HttpContext.RequestAborted,
                        };

                        serverPayload = await Shared.database_ftp.Rename(ftpModel);
                    }
                }

                if (serverPayload?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok(serverPayload);
                }
                else
                {
                    return StatusCode(500, serverPayload);
                }
            }
            catch
            {
                return StatusCode(500, serverPayload);
            }
        }
    }
}
