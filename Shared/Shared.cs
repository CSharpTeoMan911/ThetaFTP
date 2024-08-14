using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Controllers;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared
{
    public class Shared
    {
        public enum ValidationType
        {
            AccountAuthorisation,
            LogInSessionAuthorisation
        }

        public static ServerConfigModel? config;
        public static MySql mysql= new MySql();
        public static CRUD_Strategy<AuthenticationModel, AuthenticationModel, AuthenticationModel, AuthenticationModel> database_auth = new CRUD_Strategy<AuthenticationModel, AuthenticationModel, AuthenticationModel, AuthenticationModel>(new DatabaseAuthenticationController());
        public static CRUD_Strategy<string, ValidationModel, string, string> database_validation = new CRUD_Strategy<string, ValidationModel, string, string>(new DatabaseValidationController());
        public static DatabaseAuthenticationValidationController database_auth_validation = new DatabaseAuthenticationValidationController();
    }
}
