using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    public class DatabaseAuthenticationController : CRUD_Interface<AuthenticationModel, AuthenticationModel, AuthenticationModel, AuthenticationModel>
    {
        public Task<string?> Delete(AuthenticationModel? value)
        {
            throw new NotImplementedException();
        }

        public Task<string?> Get(AuthenticationModel? value)
        {
            throw new NotImplementedException();
        }

        public Task<string?> Insert(AuthenticationModel? value)
        {
            throw new NotImplementedException();
        }

        public Task<string?> Update(AuthenticationModel? value)
        {
            throw new NotImplementedException();
        }
    }
}
