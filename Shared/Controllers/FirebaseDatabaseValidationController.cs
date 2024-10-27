using Firebase.Database;
using Firebase.Database.Query;
using HallRentalSystem.Classes.StructuralAndBehavioralElements.Formaters;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data.Common;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    public class FirebaseDatabaseValidationController
    {
        public async Task<string?> ValidateAccount(ValidationModel? value)
        {

            string? response = String.Empty;
            ServerPayloadModel serverPayload = new ServerPayloadModel();
            serverPayload.response_message = "Internal server error";

            if (value?.email != null)
            {
                if (value?.code != null)
                {
                    Tuple<string, Type> hashed_key = await Sha512Hasher.Hash(value.code);

                    if (hashed_key.Item2 != typeof(Exception))
                    {
                        string base64_email = await Base64Formatter.FromUtf8ToBase64(value.email);
                        string base64_hashed_key = await Base64Formatter.FromUtf8ToBase64(hashed_key.Item1);

                        FirebaseClient? client = await Shared.firebase.Firebase();

                        if (client != null)
                        {

                            string extracted_account = await client.Child("Accounts_Waiting_For_Approval").OrderBy("key").EqualTo(base64_hashed_key).OnceAsJsonAsync();
                            Dictionary<string, FirebaseApprovalModel>? firebaseApprovalModel = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseApprovalModel>?>(extracted_account);

                            if (firebaseApprovalModel?.Keys.Count() > 0)
                            {
                                if (await Base64Formatter.FromBase64ToUtf8(firebaseApprovalModel.Values.ElementAt(0).email) == value?.email)
                                {
                                    await client.Child("Accounts_Waiting_For_Approval").Child(firebaseApprovalModel?.Keys.ElementAt(0)).DeleteAsync();

                                    string log_in_session_key = await CodeGenerator.GenerateKey(40);
                                    Tuple<string, Type> hashed_log_in_session_key = await Sha512Hasher.Hash(log_in_session_key);

                                    if (hashed_log_in_session_key.Item2 != typeof(Exception))
                                    {
                                        string base64_hashed_log_in_session_key = await Base64Formatter.FromUtf8ToBase64(hashed_log_in_session_key.Item1);

                                        FirebaseLogInSessionModel firebaseLogInSessionModel = new FirebaseLogInSessionModel()
                                        {
                                            email = base64_email,
                                            key = base64_hashed_log_in_session_key,
                                            expiry_date = Convert.ToInt64(DateTime.Now.AddDays(2).ToString("yyyyMMddHHmm"))
                                        };

                                        await client.Child("Log_In_Sessions").PostAsync(firebaseLogInSessionModel, false);

                                        serverPayload.response_message = "Account authorised";
                                        serverPayload.content = log_in_session_key;
                                    }
                                    else
                                    {
                                        serverPayload.response_message = "Internal server error";
                                    }
                                }
                                else
                                {
                                    serverPayload.response_message = "Invalid code";
                                }
                            }
                            else
                            {
                                serverPayload.response_message = "Invalid code";
                            }
                        }
                        else
                        {
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
                    serverPayload.response_message = "Invalid code";
                }
            }
            else
            {
                serverPayload.response_message = "Invalid email";
            }

            response = await JsonFormatter.JsonSerialiser(serverPayload);
            return response;
        }

        public async Task<string?> ValidateLogInSession(ValidationModel? value)
        {
            string? response = String.Empty;
            ServerPayloadModel serverPayload = new ServerPayloadModel();
            serverPayload.response_message = "Internal server error";

            if (value?.email != null)
            {
                if (value?.code != null)
                {
                    Tuple<string, Type> hashed_key = await Sha512Hasher.Hash(value.code);

                    if (hashed_key.Item2 != typeof(Exception))
                    {
                        string base64_hashed_key = await Base64Formatter.FromUtf8ToBase64(hashed_key.Item1);

                        FirebaseClient? client = await Shared.firebase.Firebase();

                        if (client != null)
                        {
                            string extracted_log_in_session = await client.Child("Log_In_Sessions_Waiting_For_Approval").OrderBy("code").EqualTo(base64_hashed_key).OnceAsJsonAsync();
                            Dictionary<string, FirebaseLogInSessionModel>? deserialised_log_in_session = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseLogInSessionModel>?>(extracted_log_in_session);

                            if (deserialised_log_in_session?.Keys.Count() > 0)
                            {
                                await client.Child("Log_In_Sessions_Waiting_For_Approval").Child(deserialised_log_in_session?.Keys.ElementAt(0)).DeleteAsync();
                                serverPayload.response_message = "Authentication successful";
                            }
                            else
                            {
                                serverPayload.response_message = "Invalid code";
                            }
                        }
                        else
                        {
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
                    serverPayload.response_message = "Invalid code";
                }
            }
            else
            {
                serverPayload.response_message = "Invalid email";
            }

            response = await JsonFormatter.JsonSerialiser(serverPayload);
            return response;
        }

        public async Task<string> ValidateLogInSessionKey(string? code)
        {
            string? response = "Internal server error";


            if (code != null)
            {
                Tuple<string, Type> hashed_key = await Sha512Hasher.Hash(code);

                if (hashed_key.Item2 != typeof(Exception))
                {
                    string base64_hashed_key = await Base64Formatter.FromUtf8ToBase64(hashed_key.Item1);

                    
                    FirebaseClient? client = await Shared.firebase.Firebase();
                    if (client != null)
                    {
                        string extracted_log_in_session = await client.Child("Log_In_Sessions").OrderBy("key").EqualTo(base64_hashed_key).OnceAsJsonAsync();
                        Dictionary<string, FirebaseLogInSessionModel>? deserialised_log_in_session = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseLogInSessionModel>?>(extracted_log_in_session);
                        string utf8_email = await Base64Formatter.FromBase64ToUtf8(deserialised_log_in_session?.Values.ElementAt(0).email);

                        if (deserialised_log_in_session?.Keys.Count() > 0)
                        {
                            if (Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm")) < deserialised_log_in_session?.Values.ElementAt(0).expiry_date)
                            {
                                if (Shared.config?.two_step_auth == true)
                                {
                                    string extracted_log_in_session_waiting_for_approval = await client.Child("Log_In_Sessions_Waiting_For_Approval").OrderBy("key").EqualTo(base64_hashed_key).OnceAsJsonAsync();
                                    Dictionary<string, FirebaseLogInSessionApprovalModel>? deserialised_log_in_session_waiting_for_approval = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseLogInSessionApprovalModel>?>(extracted_log_in_session_waiting_for_approval);

                                    if (deserialised_log_in_session_waiting_for_approval?.Keys.Count() == 0)
                                    {
                                        response = utf8_email;
                                    }
                                    else
                                    {
                                        response = "Log in session not approved";
                                    }
                                }
                                else
                                {
                                    response = utf8_email;
                                }
                            }
                            else
                            {
                                response = "Log in session key expired";
                            }
                        }
                        else
                        {
                            response = "Invalid log in session key";
                        }

                    }
                    else
                    {
                        response = "Internal server error";
                    }
                }
                else
                {
                    response = "Internal server error";
                }
            }
            else
            {
                response = "Invalid log in session key";
            }

            return response;
        }

        public async Task<string?> DeleteLogInSession(string? code)
        {
            string? response = "Internal server error";

            if (code != null)
            {
                Tuple<string, Type> hashed_key = await Sha512Hasher.Hash(code);

                if (hashed_key.Item2 != typeof(Exception))
                {
                    string base64_hashed_key = await Base64Formatter.FromUtf8ToBase64(hashed_key.Item1);

                    FirebaseClient? client = await Shared.firebase.Firebase();

                    if (client != null)
                    {
                        try
                        {
                            string extracted_log_in_session = await client.Child("Log_In_Sessions").OrderBy("key").OnceAsJsonAsync();
                            Dictionary<string, FirebaseLogInSessionModel>? deserialised_log_in_session = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseLogInSessionModel>?>(extracted_log_in_session);

                            if (deserialised_log_in_session?.Keys.Count() > 0)
                            {
                                await client.Child("Log_In_Sessions").Child(deserialised_log_in_session.Keys.ElementAt(0)).DeleteAsync();
                            }
                            else
                            {
                                response = "Invalid code";
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        response = "Internal server error";
                    }
                }
                else
                {
                    response = "Internal server error";
                }
            }
            else
            {
                response = "Invalid code";
            }

            return response;
        }
    }
}
