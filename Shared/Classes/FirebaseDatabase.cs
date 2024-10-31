namespace ThetaFTP.Shared.Classes
{
    using Firebase.Database;
    using Firebase.Auth;
    using Firebase.Auth.Providers;
    using MySqlX.XDevAPI;
    using Newtonsoft.Json;

    public class FirebaseDatabase
    {
        private FirebaseClient? firebaseClient { get; set; }

        public Task<FirebaseClient?> Firebase()
        {
            if (Shared.config != null)
            {
                if (firebaseClient == null)
                    firebaseClient = new FirebaseClient(Shared.config.firebase_database_url, new FirebaseOptions()
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

        private Task<string> AdminAuth() => Shared.config == null ? Task.FromResult(String.Empty) : Task.FromResult(Shared.config.firebase_admin_token);
    }
}
