using Google.Apis.Auth;
using System;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class GoogleAuthValidation
    {
        public async Task<GAuthModel> ValidateJwtToken(GoogleAutheticationModel? value)
        {
            GAuthModel gAuthModel = new GAuthModel();

            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(value?.jwt);

                gAuthModel.successful = (payload.EmailVerified && payload.ExpirationTimeSeconds != null && payload.Nonce == value?.nonce) ? payload.ExpirationTimeSeconds > 0 : false;
                gAuthModel.uuid = payload.Subject;

                return gAuthModel;
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return gAuthModel;
            }
        }
    }
}
