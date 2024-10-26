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
                        FirebaseClient? client = await Shared.firebase.Firebase();

                        if (client != null)
                        {
                            string base64_email = Base64Formatter.FromUtf8ToBase64(value.email);

                            string extracted_credentials = await client.Child("Credentials").OrderBy("email").EqualTo(base64_email).OnceAsJsonAsync();
                            Dictionary<string, FirebaseCredentialModel>? deserialised_credentials = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseCredentialModel>?>(extracted_credentials);

                            if (deserialised_credentials?.Keys.Count() > 0)
                            {
                                Tuple<string, Type> hashed_password = await Sha512Hasher.Hash(value?.password);

                                if (hashed_password.Item2 != typeof(Exception))
                                {
                                    string base64_hashed_password = Base64Formatter.FromUtf8ToBase64(hashed_password.Item1);

                                    if (deserialised_credentials.Values.ElementAt(0).password == base64_hashed_password)
                                    {
                                        if (Shared.config?.two_step_auth == true)
                                        {
                                            string extracted_approval = await client.Child("Accounts_Waiting_For_Approval").OrderBy("email").EqualTo(base64_email).OnceAsJsonAsync();
                                            Dictionary<string, FirebaseApprovalModel>? deserialised_approval = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseApprovalModel>?>(extracted_approval);

                                            if (deserialised_approval?.Keys.Count > 0)
                                            {
                                                serverPayload.response_message = "Account not approved";
                                                goto End;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        serverPayload.response_message = "Invalid password";
                                    }
                                }
                                else
                                {
                                    serverPayload.response_message = "Internal server error";
                                }

                                string log_in_session_key = await CodeGenerator.GenerateKey(40);
                                Tuple<string, Type> hashed_log_in_session_key = await Sha512Hasher.Hash(log_in_session_key);

                                if (hashed_log_in_session_key.Item2 != typeof(Exception))
                                {
                                    string base64_hashed_log_in_session_key = Base64Formatter.FromUtf8ToBase64(hashed_log_in_session_key.Item1);

                                    FirebaseLogInSessionModel firebaseLogInSessionModel = new FirebaseLogInSessionModel()
                                    {
                                        email = base64_email,
                                        expiry_date = Convert.ToInt64(DateTime.Now.AddDays(2).ToString("yyyyMMddHHmm")),
                                        key = base64_hashed_log_in_session_key
                                    };
                                    FirebaseObject<FirebaseLogInSessionModel> firebaseLogInSessionResult = await client.Child("Log_In_Sessions").PostAsync(firebaseLogInSessionModel, false);

                                    if (Shared.config?.two_step_auth == true)
                                    {
                                        string log_in_code = await CodeGenerator.GenerateKey(10);
                                        Tuple<string, Type> hashed_log_in_code = await Sha512Hasher.Hash(log_in_code);
                                        string base64_hashed_log_in_code = Base64Formatter.FromUtf8ToBase64(hashed_log_in_code.Item1);

                                        if (hashed_log_in_code.Item2 != typeof(Exception))
                                        {

                                            FirebaseLogInSessionApprovalModel firebaseLogInSessionWaitingForApprovalModel = new FirebaseLogInSessionApprovalModel()
                                            {
                                                code = base64_hashed_log_in_code,
                                                expiry_date = Convert.ToInt64(DateTime.Now.AddDays(2).ToString("yyyyMMddHHmm")),
                                                key = base64_hashed_log_in_session_key
                                            };

                                            FirebaseObject<FirebaseLogInSessionApprovalModel> firebaseLogInSessionWaitingForApprovalResult = await client.Child("Log_In_Sessions_Waiting_For_Approval").PostAsync(firebaseLogInSessionWaitingForApprovalModel, false);
                                            bool smtps_operation_result = SMTPS_Service.SendSMTPS(value?.email, "Log in authorisation", $"Login code: {log_in_code}");


                                            if (smtps_operation_result == true)
                                            {
                                                serverPayload.content = log_in_session_key;
                                                serverPayload.response_message = "Check the code sent to your email address to approve your log in session";
                                            }
                                            else
                                            {
                                                await client.Child("Log_In_Sessions").Child(firebaseLogInSessionResult.Key).DeleteAsync();
                                                await client.Child("Log_In_Sessions_Waiting_For_Approval").Child(firebaseLogInSessionWaitingForApprovalResult.Key).DeleteAsync();
                                                serverPayload.response_message = "Internal server error";
                                            }
                                        }
                                        else
                                        {
                                            serverPayload.response_message = "Internal server error";
                                        }
                                    }
                                    else
                                    {
                                        serverPayload.content = log_in_session_key;
                                        serverPayload.response_message = "Authentication successful";
                                    }
                                }
                                else
                                {
                                    serverPayload.response_message = "Internal server error";
                                }

                                End:;
                            }
                            else
                            {
                                serverPayload.response_message = "Invalid email";
                            }
                        }
                        else
                        {
                            serverPayload.response_message = "Internal server error";
                        }
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
                            string base64_email = Base64Formatter.FromUtf8ToBase64(value.email);

                            string? exctracted_credentials = await client.Child("Credentials").OrderBy("email").EqualTo(base64_email).OnceAsJsonAsync();
                            Dictionary<string, FirebaseCredentialModel>? deserialised_credentials = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseCredentialModel>>(exctracted_credentials);
                            if (deserialised_credentials?.Keys.Count == 0)
                            {
                                Tuple<string,Type> hashed_password = await Sha512Hasher.Hash(value.password);
                                string base64_hashed_password = Base64Formatter.FromUtf8ToBase64(hashed_password.Item1);

                                if (hashed_password.Item2 != typeof(Exception))
                                {
                                    FirebaseCredentialModel credentialModel = new FirebaseCredentialModel()
                                    {
                                        email = base64_email,
                                        password = base64_hashed_password
                                    };

                                    FirebaseObject<FirebaseCredentialModel> result = await client.Child("Credentials").PostAsync(credentialModel, false);
                                    


                                    if (Shared.config?.two_step_auth == true)
                                    {
                                        string key = await CodeGenerator.GenerateKey(10);
                                        Tuple<string, Type> key_hash_result = await Sha512Hasher.Hash(key);

                                        if (key_hash_result.Item2 != typeof(Exception))
                                        {
                                            string base64_key_hash = Base64Formatter.FromUtf8ToBase64(key_hash_result.Item1);

                                            bool smtps_operation_result = SMTPS_Service.SendSMTPS(value?.email, "Account approval", $"Account approval key: {key}");

                                            if (smtps_operation_result == true)
                                            {
                                                FirebaseApprovalModel firebaseApprovalModel = new FirebaseApprovalModel()
                                                {
                                                    email = base64_email,
                                                    key = base64_key_hash,
                                                    expiry_date = Convert.ToInt64(DateTime.Now.AddHours(1).ToString("yyyyMMddHHmm"))
                                                };

                                                await client.Child("Accounts_Waiting_For_Approval").PostAsync(firebaseApprovalModel, false);
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
                            else
                            {
                                response = "Account already exists";
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
