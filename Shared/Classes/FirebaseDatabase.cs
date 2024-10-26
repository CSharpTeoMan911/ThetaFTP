namespace ThetaFTP.Shared.Classes
{
    using Firebase.Database;
    using Firebase.Auth;
    using Firebase.Auth.Providers;
    using MySqlX.XDevAPI;

    public class FirebaseDatabase
    {
        public Task<FirebaseClient?> Firebase()
        {
            if (Shared.config != null)
            {
                FirebaseClient? client = new FirebaseClient(Shared.config.firebase__database_url, new FirebaseOptions()
                {
                    AuthTokenAsyncFactory = () => Authenticate()
                });

                return Task.FromResult((FirebaseClient?)client);
            }
            else
            {
                return Task.FromResult((FirebaseClient?)null);
            }
        }




        private async Task<string> Authenticate()
        {
            string? result = "Authentication error";

            if (Shared.config != null)
            {
                FirebaseAuthClient client = new FirebaseAuthClient(new FirebaseAuthConfig()
                {
                    ApiKey = Shared.config.firebase_api_key,
                    AuthDomain = Shared.config.firebase_auth_domain,
                    Providers = new FirebaseAuthProvider[]
                    {
                        new EmailProvider()
                    }
                });

                try
                {
                    UserCredential credentials = await client.SignInWithEmailAndPasswordAsync(Shared.config.firebase_email, Shared.config.firebase_password);
                    result = credentials.User.Credential.IdToken;
                }
                catch(Exception)
                {
                    if (typeof(Exception) == typeof(FirebaseAuthException))
                        result = "Invalid credentials";
                }
            }

            return result;
        }
    }
}
