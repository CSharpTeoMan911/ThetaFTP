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
        public async Task<ActionResult?> Insert(GoogleAutheticationModel? value)
        {
            PayloadModel payload = new PayloadModel();
            payload.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            payload.result = "Internal server error";
            payload.payload = "MOCK SESSION KEY";

            GAuthModel gAuthModel = await Shared.googleAuth.ValidateJwtToken(value);


            if (gAuthModel.successful == true)
            {
                payload.result = "Authentication Successful";
                payload.StatusCode = System.Net.HttpStatusCode.OK;

                System.Diagnostics.Debug.WriteLine($"Google JWT Token Valid");

                if (Shared.configurations?.use_firebase == true)
                {

                }
                else
                {

                }
            }


            if (payload.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return Ok(payload);
            }
            else
            {
                return StatusCode(500, payload);
            }
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
