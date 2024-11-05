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

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
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

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
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

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    result = await Shared.database_validation.DeleteLogInSession(value);
                }
                else
                {
                    result = await Shared.firebase_database_validation.DeleteLogInSession(value);
                }
            return result;
        }

        [HttpDelete("delete-account")]
        public async Task<string?> DeleteAccount([FromQuery] string? value)
        {
            string? result = String.Empty;

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    result = await Shared.database_validation.ValidateAccountDeletion(value);
                }
                else
                {
                    result = await Shared.firebase_database_validation.ValidateAccountDeletion(value);
                }
            return result;
        }

        [HttpPut("update-account")]
        public async Task<string?> UpdateAccount([FromQuery] PasswordUpdateValidationModel? value)
        {
            string? result = String.Empty;

            if (Shared.configurations != null)
                if (!Shared.configurations.use_firebase)
                {
                    result = await Shared.database_validation.ValidateAccountUpdate(value);
                }
                else
                {
                    result = await Shared.firebase_database_validation.ValidateAccountUpdate(value);
                }
            return result;
        }
    }
}
