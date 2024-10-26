using Firebase.Database;
using Firebase.Database.Query;
using HallRentalSystem.Classes.StructuralAndBehavioralElements.Formaters;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Net;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    public class FirebaseDatabaseAuthenticationController : CRUD_Interface<AuthenticationModel, string, AuthenticationModel, AuthenticationModel, string, AuthenticationModel>
    {
        public Task<string?> Delete(AuthenticationModel? value)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> Get(AuthenticationModel? value)
        {
            string? response = "Internal server error";

            ServerPayloadModel serverPayload = new ServerPayloadModel();
            serverPayload.response_message = "Internal server error";

            if (value != null)
            {
                if (value.email != null)
                {
                    if (value.password != null)
                    {

                    }
                    else
                    {
                        serverPayload.response_message = "Invalid password";
                    }
                }
                else
                {
                    serverPayload.response_message = "Invalid email";
                }
            }
            else
            {
                serverPayload.response_message = "Invalid credentials";
            }

            response = await JsonFormatter.JsonSerialiser(serverPayload);

            return response;
        }

        public Task<string?> GetInfo(string? value)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> Insert(AuthenticationModel? value)
        {
            string? response = "Internal server error";

            if (value != null)
            {
                if (value.email != null)
                {
                    if (value.password != null)
                    {
                        FirebaseClient? client = await Shared.firebase.Firebase();

                        if (client != null)
                        {
                            string? exctracted_credentials = await client.Child("Credentials").OrderBy("email").EqualTo(value.email).OnceAsJsonAsync();
                            Dictionary<string, FirebaseCredentialModel>? deserialised_credentials = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseCredentialModel>>(exctracted_credentials);
                            if (deserialised_credentials?.Keys.Count == 0)
                            {
                                Tuple<string,Type> hashed_password = await Sha512Hasher.Hash(value.password);

                                if (hashed_password.Item2 != typeof(Exception))
                                {
                                    FirebaseCredentialModel credentialModel = new FirebaseCredentialModel()
                                    {
                                        email = value.email,
                                        password = hashed_password.Item1
                                    };

                                    FirebaseObject<FirebaseCredentialModel> result = await client.Child("Credentials").PostAsync(credentialModel, false);
                                    


                                    if (Shared.config?.two_step_auth == true)
                                    {
                                        string key = await CodeGenerator.GenerateKey(10);
                                        Tuple<string, Type> key_hash_result = await Sha512Hasher.Hash(key);

                                        if (key_hash_result.Item2 != typeof(Exception))
                                        {
                                            bool smtps_operation_result = SMTPS_Service.SendSMTPS(value?.email, "Account approval", $"Account approval key: {key}");

                                            if (smtps_operation_result == true)
                                            {
                                                FirebaseApprovalModel firebaseApprovalModel = new FirebaseApprovalModel()
                                                {
                                                    email = value?.email,
                                                    key = key_hash_result.Item1,
                                                    expiry_date = Convert.ToInt64(DateTime.Now.AddHours(1).ToString("yyyyMMddHHmm"))
                                                };
                                                await client.Child("Accounts_Waiting_For_Approval").PostAsync(firebaseApprovalModel);
                                                response = "Registration successful";
                                            }
                                            else
                                            {
                                                await client.Child("Credentials").Child(result.Key).DeleteAsync();
                                                response = "Internal server error";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        response = "Registration successful";
                                    }
                                }
                                else
                                {
                                    response = "Internal server error";
                                }
                            }
                        }
                        else
                        {
                            response = "Internal server error";
                        }
                    }
                    else
                    {
                        response = "Invalid password";
                    }
                }
                else
                {
                    response = "Invalid email";
                }
            }
            else
            {
                response = "Invalid credentials";
            }

            return response;
        }

        public Task<string?> Rename(string? value)
        {
            throw new NotImplementedException();
        }

        public Task<string?> Update(AuthenticationModel? value)
        {
            throw new NotImplementedException();
        }
    }
}
