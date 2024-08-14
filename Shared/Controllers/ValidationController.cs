using Microsoft.AspNetCore.Mvc;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace ThetaFTP.Shared.Controllers
{
    [Route("/validation")]
    public class ValidationController : Controller, CRUD_Interface<string, ValidationModel, string, string>
    {

        [HttpGet("validate-code")]
        public async Task<string?> Get(ValidationModel? value)
        {
            string? result = String.Empty;
            result = await Shared.database_validation.Get(value);
            return result;
        }

        public Task<string?> Insert(string? value)
        {
            throw new NotImplementedException();
        }

        public Task<string?> Update(string? value)
        {
            throw new NotImplementedException();
        }

        public Task<string?> Delete(string? value)
        {
            throw new NotImplementedException();
        }
    }
}
