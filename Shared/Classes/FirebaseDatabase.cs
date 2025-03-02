using Firebase.Database;

namespace ThetaFTP.Shared.Classes
{
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

        private Task<string> AdminAuth() => configurations == null ? Task.FromResult(String.Empty) : Task.FromResult(configurations.firebase_admin_token);
    }
}
