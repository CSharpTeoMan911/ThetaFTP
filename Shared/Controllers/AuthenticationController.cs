using Microsoft.AspNetCore.Mvc;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;


namespace ThetaFTP.Shared.Controllers
{
    [Route("/authentication")]
    [ApiController]
    public class AuthenticationController : Controller, CRUD_Interface<AuthenticationModel, string, AuthenticationModel, AuthenticationModel, string, AuthenticationModel>
    {
        [HttpDelete("delete")]
        public async Task<string?> Delete([FromQuery] AuthenticationModel? value)
        {
            string? response = "Internal server error";

            if (Shared.config != null)
                if (!Shared.config.use_firebase)
                {
                    response = await Shared.database_auth.Delete(value);
                }
                else
                {
                    //response = await Shared.database_auth.Delete(value);
                }
            return response;
        }

        [HttpGet("get")]
        public async Task<string?> Get([FromQuery] AuthenticationModel? value)
        {
            string? response = "Internal server error";

            if (Shared.config != null)
                if (!Shared.config.use_firebase)
                {
                    response = await Shared.database_auth.Get(value);
                }
                else
                {
                    response = await Shared.firebase_database_auth.Get(value);
                }
            return response;
        }

        public Task<string?> GetInfo(string? value)
        {
            throw new NotImplementedException();
        }

        [HttpPost("insert")]
        public async Task<string?> Insert([FromQuery] AuthenticationModel? value)
        {
            string? response = "Internal server error";

            if (Shared.config != null)
                if (!Shared.config.use_firebase)
                {
                    response = await Shared.database_auth.Insert(value);
                }
                else
                {
                    response = await Shared.firebase_database_auth.Insert(value);
                }
            return response;
        }

        public Task<string?> Rename(string? value)
        {
            throw new NotImplementedException();
        }

        [HttpPut("update")]
        public Task<string?> Update([FromQuery] AuthenticationModel? value)
        {
            throw new NotImplementedException();
        }
    }
}
