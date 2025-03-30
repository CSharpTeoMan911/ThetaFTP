using Google.Apis.Auth;

namespace ThetaFTP.Shared.Classes
{
    public class GoogleAuthValidation
    {
        public async Task<bool> ValidateJwtToken(string obj)
        {
            try
            {
                //await GoogleJsonWebSignature.ValidateAsync();
            }
            catch { }

            return true;
        }
    }
}
