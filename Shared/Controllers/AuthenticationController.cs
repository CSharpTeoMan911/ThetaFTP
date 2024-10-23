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
            string? response = await Shared.database_auth.Delete(value);
            return response;
        }

        [HttpGet("get")]
        public async Task<string?> Get([FromQuery] AuthenticationModel? value)
        {
            string? response = await Shared.database_auth.Get(value);
            return response;
        }

        public Task<string?> GetInfo(string? value)
        {
            throw new NotImplementedException();
        }

        [HttpPost("insert")]
        public async Task<string?> Insert([FromQuery] AuthenticationModel? value)
        {
            string? response = await Shared.database_auth.Insert(value);
            return response;
        }

        public Task<string?> Rename(string? value)
        {
            throw new NotImplementedException();
        }

        [HttpPut("update")]
        public async Task<string?> Update([FromQuery] AuthenticationModel? value)
        {
            string? response = String.Empty;

            return response;
        }
    }
}
