namespace ThetaFTP.Shared.Classes
{
    using Firebase.Database;
    using Firebase.Auth;
    using Firebase.Auth.Providers;
    using MySqlX.XDevAPI;
    using Newtonsoft.Json;

    public class FirebaseDatabase
    {
        private FirebaseClient? firebaseClient;

        public Task<FirebaseClient?> Firebase()
        {
            if (Shared.config != null)
            {
                if (firebaseClient == null)
                    firebaseClient = new FirebaseClient(Shared.config.firebase__database_url, new FirebaseOptions()
                    {
                        AuthTokenAsyncFactory = () => Authenticate(),
                        
                    });

                return Task.FromResult((FirebaseClient?)firebaseClient);
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
