using System.Runtime.CompilerServices;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Controllers;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared
{
    public class Shared
    {
        private static bool AesLoaded { get; set; }
        private static AesFileEncryption? aes;

        public static void SetAes(AesFileEncryption? _aes)
        {
            if(AesLoaded == false)
            {
                AesLoaded = true;
                aes = _aes;
            }
        }

        public static GoogleAuthValidation googleAuth = new GoogleAuthValidation();

        public static AesFileEncryption? GetAes() => aes;

        public const string HttpClientConfig = "Default";
        public static Sha512Hasher? sha512 { get; set; }
        public static ServerConfigModel? configurations { get; set; }
        public static Classes.MySql mysql= new Classes.MySql();
        public static FirebaseDatabase firebase = new FirebaseDatabase();

        public static CRUD_Payload_Strategy<FtpModel, Metadata, FtpModel, FtpModel, FtpModel, FtpModel> database_ftp = new CRUD_Payload_Strategy<FtpModel, Metadata, FtpModel, FtpModel, FtpModel, FtpModel>(new FtpDatabaseController());
        public static CRUD_Payload_Strategy<FtpDirectoryModel, Metadata, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel> database_directory_ftp = new CRUD_Payload_Strategy<FtpDirectoryModel, Metadata, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel, FtpDirectoryModel>(new FtpDirectoryDatabaseController());

        public static CRUD_Auth_Strategy<AuthenticationModel, string, AuthenticationModel, PasswordUpdateModel, string, string> firebase_database_auth = new CRUD_Auth_Strategy<AuthenticationModel, string, AuthenticationModel, PasswordUpdateModel, string, string>(new FirebaseDatabaseAuthenticationController());
        public static CRUD_Auth_Strategy<AuthenticationModel, string, AuthenticationModel, PasswordUpdateModel, string, string> database_auth = new CRUD_Auth_Strategy<AuthenticationModel, string, AuthenticationModel, PasswordUpdateModel, string, string>(new DatabaseAuthenticationController());

        public static CRUD_Auth_Strategy<string, string, string, string, string, string> google_auth_database = new CRUD_Auth_Strategy<string, string, string, string, string, string>(new GoogleAuthenticationDatabaseController());

        public static DatabaseServerFunctionsController databaseServerFunctions = new DatabaseServerFunctionsController();
        public static FirebaseDatabaseServerFunctionsController fireabseDatabaseServerFunctions = new FirebaseDatabaseServerFunctionsController();
        public static DatabaseValidationController database_validation = new DatabaseValidationController();
        public static FirebaseDatabaseValidationController firebase_database_validation = new FirebaseDatabaseValidationController();

    }
}
