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
            string? respose = String.Empty;

            return respose;
        }

        [HttpGet("get")]
        public async Task<string?> Get([FromQuery] AuthenticationModel? value)
        {
            string? respose = String.Empty;

            return respose;
        }

        [HttpPost("insert")]
        public async Task<string?> Insert([FromQuery] AuthenticationModel? value)
        {
            string? respose = String.Empty;

            return respose;
        }

        [HttpPut("update")]
        public async Task<string?> Update([FromQuery] AuthenticationModel? value)
        {
            string? respose = String.Empty;

            return respose;
        }
    }
}
