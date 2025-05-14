using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    [EnableRateLimiting("sliding_window")]
    [Route("/google-auth")]
    public class GoogleAuthenticationController : Controller, CRUD_Auth_Api_Interface<GoogleAutheticationModel, string, GoogleAutheticationModel, string, string, string> 
    {
        public Task<ActionResult?> Delete(string? value)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult?> Get(GoogleAutheticationModel? value)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult?> GetInfo(string? value)
        {
            throw new NotImplementedException();
        }

        [HttpPost("auth")]
        public Task<ActionResult?> Insert(GoogleAutheticationModel? value)
        {
            System.Diagnostics.Debug.WriteLine($"Token: {value?.jwt}");



            return Task.FromResult<ActionResult?>(Ok(""));
        }

        public Task<ActionResult?> Rename(string? value)
        {
            throw new NotImplementedException();
        }

        public Task<ActionResult?> Update(string? value)
        {
            throw new NotImplementedException();
        }
    }
}
