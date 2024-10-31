using System.Collections.Concurrent;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Controllers;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared
{
    public class Shared
    {
        public const string HttpClientConfig = "Default";
        public static ServerConfigModel? config;
        public static Classes.MySql mysql= new Classes.MySql();
        public static FirebaseDatabase firebase = new FirebaseDatabase();
        public static CRUD_Strategy<AuthenticationModel, string, AuthenticationModel, AuthenticationModel, string, AuthenticationModel> firebase_database_auth = new CRUD_Strategy<AuthenticationModel, string, AuthenticationModel, AuthenticationModel, string, AuthenticationModel>(new FirebaseDatabaseAuthenticationController());
        public static CRUD_Strategy<AuthenticationModel, string, AuthenticationModel, AuthenticationModel, string, AuthenticationModel> database_auth = new CRUD_Strategy<AuthenticationModel, string, AuthenticationModel, AuthenticationModel, string, AuthenticationModel>(new DatabaseAuthenticationController());
        public static CRUD_Strategy<FtpModel, Metadata, FtpModel, FtpModel, FtpModel, FtpModel> database_ftp = new CRUD_Strategy<FtpModel, Metadata, FtpModel, FtpModel, FtpModel, FtpModel>(new FtpDatabaseController());
        public static CRUD_Strategy<FtpDirectoryModel, Metadata, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel> database_directory_ftp = new CRUD_Strategy<FtpDirectoryModel, Metadata, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel>(new FtpDirectoryDatabaseController());
        public static DatabaseServerFunctionsController databaseServerFunctions = new DatabaseServerFunctionsController();
        public static DatabaseValidationController database_validation = new DatabaseValidationController();
        public static FirebaseDatabaseValidationController firebase_database_validation = new FirebaseDatabaseValidationController();
    }
}
