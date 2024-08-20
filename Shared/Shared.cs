using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Controllers;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared
{
    public class Shared
    {
        public static ServerConfigModel? config;
        public static MySql mysql= new MySql();
        public static CRUD_Strategy<AuthenticationModel, AuthenticationModel, AuthenticationModel, AuthenticationModel> database_auth = new CRUD_Strategy<AuthenticationModel, AuthenticationModel, AuthenticationModel, AuthenticationModel>(new DatabaseAuthenticationController());
        public static DatabaseValidationController database_validation = new DatabaseValidationController();
        public static DatabaseAuthenticationValidationController database_auth_validation = new DatabaseAuthenticationValidationController();
    }
}
