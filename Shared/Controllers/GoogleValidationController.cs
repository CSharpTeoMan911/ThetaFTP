using Microsoft.AspNetCore.Mvc;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    [Route("/g_validation")]
    public class GoogleValidationController : Controller
    {
        [HttpGet("validate-code")]
        public async Task<ActionResult?> GetLogInCode([FromQuery] ValidationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    payloadModel = await Shared.googleValidationDatabase.ValidateLogInSession(value);
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
    }
}
