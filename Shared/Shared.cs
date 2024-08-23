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
        public static CRUD_Strategy<FtpModel, FtpModel, FtpModel, FtpModel> database_ftp = new CRUD_Strategy<FtpModel, FtpModel, FtpModel, FtpModel>(new FtpDatabaseController());
        public static CRUD_Strategy<FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel> database_directory_ftp = new CRUD_Strategy<FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel>(new FtpDirectoryDatabaseController());
        public static DatabaseServerFunctionsController databaseServerFunctions = new DatabaseServerFunctionsController();
        public static DatabaseValidationController database_validation = new DatabaseValidationController();
    }
}
