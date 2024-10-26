using HallRentalSystem.Classes.StructuralAndBehavioralElements.Formaters;
using Microsoft.AspNetCore.Mvc;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace ThetaFTP.Shared.Controllers
{
    [Route("/validation")]
    [ApiController]
    public class ValidationController : Controller
    {
        [HttpGet("validate-account")]
        public async Task<string?> GetAccount([FromQuery] ValidationModel? value)
        {
            string? result = String.Empty;

            if (Shared.config != null)
                if (!Shared.config.use_firebase)
                {
                    result = await Shared.database_validation.ValidateAccount(value);
                }
                else
                {
                    result = await Shared.firebase_database_validation.ValidateAccount(value);
                }
            return result;
        }

        [HttpGet("validate-code")]
        public async Task<string?> GetLogInCode([FromQuery] ValidationModel? value)
        {
            string? result = String.Empty;

            if (Shared.config != null)
                if (!Shared.config.use_firebase)
                {
                    result = await Shared.database_validation.ValidateLogInSession(value);
                }
                else
                {
                    result = await Shared.firebase_database_validation.ValidateLogInSession(value);
                }
            return result;
        }

        [HttpDelete("delete-session")]
        public async Task<string?> DeleteLogInSession([FromQuery] string? value)
        {
            string? result = String.Empty;

            if (Shared.config != null)
                if (!Shared.config.use_firebase)
                {
                    result = await Shared.database_validation.DeleteLogInSession(value);
                }
                else
                {
                    result = await Shared.firebase_database_validation.DeleteLogInSession(value);
                }
            return result;
        }
    }
}
