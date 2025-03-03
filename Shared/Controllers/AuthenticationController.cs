using LiteDB;
using Microsoft.AspNetCore.Mvc;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;
using static Google.Rpc.Context.AttributeContext.Types;


namespace ThetaFTP.Shared.Controllers
{
    [Route("/authentication")]
    [ApiController]
    public class AuthenticationController : Controller, CRUD_Auth_Api_Interface<AuthenticationModel, string, AuthenticationModel, PasswordUpdateModel, string, string>
    {
        [HttpDelete("delete")]
        public async Task<ActionResult?> Delete([FromQuery] string? value)
        {
            PayloadModel? payloadModel = new PayloadModel();


            if (Shared.configurations != null)
            {
                if (!Shared.configurations.use_firebase)
                    payloadModel = await Shared.database_validation.ValidateLogInSessionKey(value);
                else
                    payloadModel = await Shared.firebase_database_validation.ValidateLogInSessionKey(value);

                if (payloadModel?.StatusCode == System.Net.HttpStatusCode.OK && payloadModel?.payload?.GetType() == typeof(string))
                {
                    if (!Shared.configurations.use_firebase)
                    {
                        payloadModel = await Shared.database_auth.Delete((string)payloadModel.payload);
                    }
                    else
                    {
                        payloadModel = await Shared.firebase_database_auth.Delete((string)payloadModel.payload);
                    }
                }
            }

            if (payloadModel?.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(payloadModel);
            }
            else
            {
                return StatusCode(500, payloadModel);
            }
        }

        [HttpGet("get")]
        public async Task<ActionResult?> Get([FromQuery] AuthenticationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    payloadModel = await Shared.database_auth.Get(value);
                }
                else
                {
                    payloadModel = await Shared.firebase_database_auth.Get(value);
                }

            if (payloadModel?.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(payloadModel);
            }
            else
            {
                return StatusCode(500, payloadModel);
            }
        }

        [HttpGet("get-info")]
        public async Task<ActionResult?> GetInfo([FromQuery]string? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                    payloadModel = await Shared.database_validation.ValidateLogInSessionKey(value);
                else
                    payloadModel = await Shared.firebase_database_validation.ValidateLogInSessionKey(value);

            if (payloadModel?.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(payloadModel);
            }
            else
            {
                return StatusCode(500, payloadModel);
            }
        }

        [HttpPost("insert")]
        public async Task<ActionResult?> Insert([FromQuery] AuthenticationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    payloadModel = await Shared.database_auth.Insert(value);
                }
                else
                {
                    payloadModel = await Shared.firebase_database_auth.Insert(value);
                }

            if (payloadModel?.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(payloadModel);
            }
            else
            {
                return StatusCode(500, payloadModel);
            }
        }

        [HttpPut("rename")]
        public Task<ActionResult?> Rename(string? value)
        {
            throw new NotImplementedException();
        }

        [HttpPut("update")]
        public async Task<ActionResult?> Update([FromQuery] PasswordUpdateModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (value != null && Shared.configurations != null)
            {
                if (!Shared.configurations.use_firebase)
                {
                    payloadModel = await Shared.database_auth.Update(value);
                }
                else
                {
                    payloadModel = await Shared.firebase_database_auth.Update(value);
                }
            }

            if (payloadModel?.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(payloadModel);
            }
            else
            {
                return StatusCode(500, payloadModel);
            }
        }

    }
}
