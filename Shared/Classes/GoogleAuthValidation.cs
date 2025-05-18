using Google.Apis.Auth;

namespace ThetaFTP.Shared.Classes
{
    public class GoogleAuthValidation
    {
        public async Task<bool> ValidateJwtToken(string jwt)
        {
            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(jwt);
                return (payload.EmailVerified && payload.ExpirationTimeSeconds != null) ? payload.ExpirationTimeSeconds > 0 : false;
            }
            catch 
            {
                return false;
            }
        }
    }
}
