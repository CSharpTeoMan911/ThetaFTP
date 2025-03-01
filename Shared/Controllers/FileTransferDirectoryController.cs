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
                        FtpDirectoryModel ftpModel = new FtpDirectoryModel()
                        {
                            email = (string)serverPayload.payload,
                            directory_name = query?.directory_name,
                            path = query?.path
                        };

                        serverPayload = await Shared.database_directory_ftp.Delete(ftpModel);
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
        public Task<ActionResult?> Get([FromQuery] DirectoryOperationMetadata? query, string? body)
        {
            throw new NotImplementedException();
        }

        [HttpGet("get-directories")]
        public async Task<ActionResult?> GetInfo([FromQuery] Metadata? query)
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
                        serverPayload = await Shared.database_directory_ftp.GetInfo(query);
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
        public async Task<ActionResult?> Insert([FromQuery] DirectoryOperationMetadata? query, [FromBody] string? body)
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
                        FtpDirectoryModel directoryModel = new FtpDirectoryModel()
                        {
                            email = (string)serverPayload.payload,
                            directory_name = query?.directory_name,
                            path = query?.path,
                        };

                        serverPayload = await Shared.database_directory_ftp.Insert(directoryModel);
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
        public async Task<ActionResult?> Update([FromQuery]DirectoryOperationMetadata? query, [FromBody]string? body)
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
                        FtpDirectoryModel directoryModel = new FtpDirectoryModel()
                        {
                            email = (string)serverPayload.payload,
                            directory_name = query?.directory_name,
                            path = query?.path,
                            new_path = query?.new_path,
                        };

                        serverPayload = await Shared.database_directory_ftp.Update(directoryModel);
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
        public async Task<ActionResult?> UpdateName([FromQuery] DirectoryOperationMetadata? query, string? body)
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
                        FtpDirectoryModel directoryModel = new FtpDirectoryModel()
                        {
                            email = (string)serverPayload.payload,
                            directory_name = query?.directory_name,
                            directory_new_name = query?.new_directory_name,
                            path = query?.path,
                        };

                        serverPayload = await Shared.database_directory_ftp.Rename(directoryModel);
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
