using Microsoft.AspNetCore.Mvc;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;


namespace ThetaFTP.Shared.Controllers
{
    [Route("/authentication")]
    public class AuthenticationController : Controller, CRUD_Interface<AuthenticationModel, AuthenticationModel, AuthenticationModel, AuthenticationModel>
    {
        [HttpDelete("delete")]
        public async Task<string?> Delete([FromQuery] AuthenticationModel? value)
        {
            string? response = await Shared.database_auth.Get(value);
            return response;
        }

        [HttpGet("get")]
        public async Task<string?> Get([FromQuery] AuthenticationModel? value)
        {
            string? response = await Shared.database_auth.Get(value);
            return response;
        }

        [HttpPost("insert")]
        public async Task<string?> Insert([FromQuery] AuthenticationModel? value)
        {
            string? response = await Shared.database_auth.Insert(value);
            return response;
        }

        [HttpPut("update")]
        public async Task<string?> Update([FromQuery] AuthenticationModel? value)
        {
            string? response = String.Empty;

            return response;
        }

        [HttpPut("verify-account")]
        public async Task<string?> VerifyAccount([FromQuery] string code)
        {
            string? response = String.Empty;

            return response;
        }

        [HttpPut("verify-account")]
        public async Task<string?> VerifyLogin([FromQuery] string code)
        {
            string? response = String.Empty;

            return response;
        }
    }
}
