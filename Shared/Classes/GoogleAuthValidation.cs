using Google.Apis.Auth;

namespace ThetaFTP.Shared.Classes
{
    public class GoogleAuthValidation
    {
        public Task<bool> ValidateJwtToken(string obj)
        {
            try
            {
                //await GoogleJsonWebSignature.ValidateAsync();
            }
            catch { }

            throw new NotImplementedException();
        }
    }
}
