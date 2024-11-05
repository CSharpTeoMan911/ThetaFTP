using LiteDB;
using Microsoft.AspNetCore.Mvc;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;


namespace ThetaFTP.Shared.Controllers
{
    [Route("/authentication")]
    [ApiController]
    public class AuthenticationController : Controller, CRUD_Interface<AuthenticationModel, string, AuthenticationModel, PasswordUpdateModel, string, string>
    {
        [HttpDelete("delete")]
        public async Task<string?> Delete([FromQuery] string? value)
        {
            string? result = "Internal server error";

            string? log_in_key_validation_result = "Internal server error";

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                    log_in_key_validation_result = await Shared.database_validation.ValidateLogInSessionKey(value);
                else
                    log_in_key_validation_result = await Shared.firebase_database_validation.ValidateLogInSessionKey(value);


            if (log_in_key_validation_result != "Internal server error")
            {
                if (log_in_key_validation_result != "Invalid log in session key")
                {
                    if (log_in_key_validation_result != "Log in session key expired")
                    {
                        if (log_in_key_validation_result != "Log in session not approved")
                        {

                            if (Shared.configurations != null)
                                if (!Shared.configurations.use_firebase)
                                {
                                    result = await Shared.database_auth.Delete(log_in_key_validation_result);
                                }
                                else
                                {
                                    result = await Shared.firebase_database_auth.Delete(log_in_key_validation_result);
                                }

                            return result;
                        }
                        else
                        {
                            return log_in_key_validation_result;
                        }
                    }
                    else
                    {
                        return log_in_key_validation_result;
                    }
                }
                else
                {
                    return log_in_key_validation_result;
                }
            }
            else
            {
                return log_in_key_validation_result;
            }
        }

        [HttpGet("get")]
        public async Task<string?> Get([FromQuery] AuthenticationModel? value)
        {
            string? response = "Internal server error";

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    response = await Shared.database_auth.Get(value);
                }
                else
                {
                    response = await Shared.firebase_database_auth.Get(value);
                }
            return response;
        }

        [HttpGet("get-info")]
        public async Task<string?> GetInfo([FromQuery]string? value)
        {
            string? result = "Internal server error";

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                    result = await Shared.database_validation.ValidateLogInSessionKey(value);
                else
                    result = await Shared.firebase_database_validation.ValidateLogInSessionKey(value);

            return result;
        }

        [HttpPost("insert")]
        public async Task<string?> Insert([FromQuery] AuthenticationModel? value)
        {
            string? response = "Internal server error";

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    response = await Shared.database_auth.Insert(value);
                }
                else
                {
                    response = await Shared.firebase_database_auth.Insert(value);
                }
            return response;
        }

        [HttpPut("rename")]
        public Task<string?> Rename(string? value)
        {
            throw new NotImplementedException();
        }

        [HttpPut("update")]
        public async Task<string?> Update([FromQuery] PasswordUpdateModel? value)
        {
            string? result = "Internal server error";

            string? log_in_key_validation_result = "Internal server error";

            if (value != null)
                if (Shared.configurations != null)
                    if (!Shared.configurations.use_firebase)
                        log_in_key_validation_result = await Shared.database_validation.ValidateLogInSessionKey(value.log_in_session_key);
                    else
                        log_in_key_validation_result = await Shared.firebase_database_validation.ValidateLogInSessionKey(value.log_in_session_key);

            if (log_in_key_validation_result != "Internal server error")
            {
                if (log_in_key_validation_result != "Invalid log in session key")
                {
                    if (log_in_key_validation_result != "Log in session key expired")
                    {
                        if (log_in_key_validation_result != "Log in session not approved")
                        {
                            if(value != null)
                                value.email = log_in_key_validation_result;

                            if (Shared.configurations != null)
                                if (!Shared.configurations.use_firebase)
                                {
                                    result = await Shared.database_auth.Update(value);
                                }
                                else
                                {
                                    result = await Shared.firebase_database_auth.Update(value);
                                }
                            return result;
                        }
                        else
                        {
                            return log_in_key_validation_result;
                        }
                    }
                    else
                    {
                        return log_in_key_validation_result;
                    }
                }
                else
                {
                    return log_in_key_validation_result;
                }
            }
            else
            {
                return log_in_key_validation_result;
            }
        }
    }
}
