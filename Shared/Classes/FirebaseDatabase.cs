namespace ThetaFTP.Shared.Classes
{
    using Firebase.Database;
    using Firebase.Auth;
    using Firebase.Auth.Providers;
    using MySqlX.XDevAPI;
    using Newtonsoft.Json;

    public class FirebaseDatabase:Shared
    {
        private FirebaseClient? firebaseClient { get; set; }

        public Task<FirebaseClient?> Firebase()
        {
            if (Shared.configurations != null)
            {
                if (firebaseClient == null)
                    firebaseClient = new FirebaseClient(configurations.firebase_database_url, new FirebaseOptions()
                    {
                        AuthTokenAsyncFactory = ()=> AdminAuth(),
                    });

               return Task.FromResult((FirebaseClient?)firebaseClient);
            }
            else
            {
                return Task.FromResult((FirebaseClient?)null);
            }
        }

        private Task<string> AdminAuth() => configurations == null || credentials == null ? Task.FromResult(String.Empty) : Task.FromResult(credentials.firebase_admin_token);
    }
}
