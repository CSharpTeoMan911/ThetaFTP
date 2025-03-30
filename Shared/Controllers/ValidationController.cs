using ThetaFTP.Shared.Formatters;
using Microsoft.AspNetCore.Mvc;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Microsoft.AspNetCore.RateLimiting;

namespace ThetaFTP.Shared.Controllers
{
    [Route("/validation")]
    [ApiController]
    public class ValidationController : Controller
    {
        [HttpGet("validate-account")]
        public async Task<ActionResult?> GetAccount([FromQuery] ValidationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    payloadModel = await Shared.database_validation.ValidateAccount(value);
                }
                else
                {
                    payloadModel = await Shared.firebase_database_validation.ValidateAccount(value);
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

        [HttpGet("validate-code")]
        public async Task<ActionResult?> GetLogInCode([FromQuery] ValidationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    payloadModel = await Shared.database_validation.ValidateLogInSession(value);
                }
                else
                {
                    payloadModel = await Shared.firebase_database_validation.ValidateLogInSession(value);
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

        [HttpDelete("delete-session")]
        public async Task<ActionResult?> DeleteLogInSession([FromQuery] string? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    payloadModel = await Shared.database_validation.DeleteLogInSession(value);
                }
                else
                {
                    payloadModel = await Shared.firebase_database_validation.DeleteLogInSession(value);
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

        [HttpDelete("delete-account")]
        public async Task<ActionResult?> DeleteAccount([FromQuery] string? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    payloadModel = await Shared.database_validation.ValidateAccountDeletion(value);
                }
                else
                {
                    payloadModel = await Shared.firebase_database_validation.ValidateAccountDeletion(value);
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

        [HttpPut("update-account")]
        public async Task<ActionResult?> UpdateAccount([FromQuery] PasswordUpdateValidationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();
            string? result = String.Empty;

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    payloadModel = await Shared.database_validation.ValidateAccountUpdate(value);
                }
                else
                {
                    payloadModel = await Shared.firebase_database_validation.ValidateAccountUpdate(value);
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
