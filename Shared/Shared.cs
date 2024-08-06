using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Controllers;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared
{
    public class Shared
    {
        public static CRUD_Strategy<AuthenticationModel, AuthenticationModel, AuthenticationModel, AuthenticationModel> database_auth = new CRUD_Strategy<AuthenticationModel, AuthenticationModel, AuthenticationModel, AuthenticationModel>(new DatabaseAuthenticationController());
    }
}
