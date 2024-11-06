using Firebase.Database;
using Firebase.Database.Query;
using HallRentalSystem.Classes.StructuralAndBehavioralElements.Formaters;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Serilog;
using System.Data.Common;
using System.Text;
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

            try
            {
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

                                        string? log_in_session_key = await CodeGenerator.GenerateKey(40);
                                        Tuple<string, Type> hashed_log_in_session_key = await Sha512Hasher.Hash(log_in_session_key);

                                        if (hashed_log_in_session_key.Item2 != typeof(Exception) && log_in_session_key != null)
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
            }
            catch (Exception e)
            {
                Log.Error(e, "Account validation API error");
                serverPayload.response_message = "Internal server error";
            }

            response = await JsonFormatter.JsonSerialiser(serverPayload);
            return response;
        }

        public async Task<string?> ValidateLogInSession(ValidationModel? value)
        {
            string? response = String.Empty;
            ServerPayloadModel serverPayload = new ServerPayloadModel();
            serverPayload.response_message = "Internal server error";

            try
            {
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
            }
            catch (Exception e)
            {
                Log.Error(e, "Log in session validation API error");
                serverPayload.response_message = "Internal server error";
            }

            response = await JsonFormatter.JsonSerialiser(serverPayload);
            return response;
        }

        public async Task<string> ValidateLogInSessionKey(string? code)
        {
            string? response = "Internal server error";

            try
            {
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

                            if (deserialised_log_in_session?.Keys.Count() > 0)
                            {
                                string utf8_email = await Base64Formatter.FromBase64ToUtf8(deserialised_log_in_session?.Values.ElementAt(0).email);

                                if (Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm")) < deserialised_log_in_session?.Values.ElementAt(0).expiry_date)
                                {
                                    if (Shared.configurations?.two_step_auth == true)
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
            }
            catch (Exception e)
            {
                Log.Error(e, "Log in session key validation API error");
                response = "Internal server error";
            }

            return response;
        }

        public async Task<string?> DeleteLogInSession(string? code)
        {
            string? response = "Internal server error";

            try
            {
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
                                string extracted_log_in_session = await client.Child("Log_In_Sessions").OrderBy("key").EqualTo(base64_hashed_key).OnceAsJsonAsync();
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
            }
            catch (Exception e)
            {
                Log.Error(e, "Account log out API error");
                response = "Internal server error";
            }

            return response;
        }


        public async Task<string?> ValidateAccountDeletion(string? code)
        {
            string? response = "Internal server error";

            try
            {
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
                                string extracted_account_deletion = await client.Child("Accounts_Waiting_For_Deletion").OrderBy("key").EqualTo(base64_hashed_key).OnceAsJsonAsync();
                                Dictionary<string, FirebaseApprovalModel>? deserialised_account_deletion = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseApprovalModel>?>(extracted_account_deletion);

                                if (deserialised_account_deletion?.Keys.Count() > 0)
                                {
                                    FirebaseApprovalModel account = deserialised_account_deletion.Values.ElementAt(0);
                                    await client.Child("Accounts_Waiting_For_Deletion").Child(deserialised_account_deletion.Keys.ElementAt(0)).DeleteAsync();

                                    if (Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm")) < account.expiry_date)
                                    {
                                        response = await DeleteAccount(account?.email);
                                    }
                                    else
                                    {
                                        response = "Account deletion code expired";
                                    }
                                }
                                else
                                {
                                    response = "Invalid code";
                                }
                            }
                            catch
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
                        response = "Internal server error";
                    }
                }
                else
                {
                    response = "Invalid code";
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Account deletion validation API error");
                response = "Internal server error";
            }

            return response;
        }


        private async Task<string> DeleteAccount(string? base64_email)
        {
            string? response = "Internal server error";

            try
            {
                FirebaseClient? client = await Shared.firebase.Firebase();

                if (client != null)
                {
                    string email = await Base64Formatter.FromBase64ToUtf8(base64_email);
                    string? extracted_credentials = await client.Child("Credentials").OrderBy("email").EqualTo(base64_email).OnceAsJsonAsync();
                    Dictionary<string, FirebaseCredentialModel>? deserialised_credentials = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseCredentialModel>>(extracted_credentials);


                    if (deserialised_credentials?.Keys.Count > 0)
                    {
                        StringBuilder pathBuilder = new StringBuilder(Environment.CurrentDirectory);
                        pathBuilder.Append(FileSystemFormatter.PathSeparator());
                        pathBuilder.Append("FTP_Server");
                        pathBuilder.Append(FileSystemFormatter.PathSeparator());
                        pathBuilder.Append(email);


                        FileSystemFormatter.DeleteDirectory(pathBuilder.ToString());

                        await client.Child("Credentials").Child(deserialised_credentials?.Keys.ElementAt(0)).DeleteAsync();

                        string? extracted_log_in_sessions = await client.Child("Log_In_Sessions").OrderBy("email").EqualTo(base64_email).OnceAsJsonAsync();
                        Dictionary<string, FirebaseLogInSessionModel>? deserialised_log_in_sessions = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseLogInSessionModel>>(extracted_log_in_sessions);

                        for (int i = 0; i < deserialised_log_in_sessions?.Keys.Count(); i++)
                        {
                            await client.Child("Log_In_Sessions").Child(deserialised_log_in_sessions.Keys.ElementAt(i)).DeleteAsync();

                            string? extracted_log_in_sessions_waiting_for_approval = await client.Child("Log_In_Sessions_Waiting_For_Approval").OrderBy("key").EqualTo(deserialised_log_in_sessions.Values.ElementAt(i).key).OnceAsJsonAsync();
                            Dictionary<string, FirebaseLogInSessionApprovalModel>? deserialised_extracted_log_in_sessions_waiting_for_approval = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseLogInSessionApprovalModel>>(extracted_log_in_sessions_waiting_for_approval);

                            if (deserialised_extracted_log_in_sessions_waiting_for_approval?.Keys.Count() > 0)
                                await client.Child("Log_In_Sessions_Waiting_For_Approval").Child(deserialised_extracted_log_in_sessions_waiting_for_approval?.Keys.ElementAt(0)).DeleteAsync();
                        }


                        string? extracted_accounts_waiting_for_approval = await client.Child("Accounts_Waiting_For_Approval").OrderBy("email").EqualTo(base64_email).OnceAsJsonAsync();
                        Dictionary<string, FirebaseApprovalModel>? deserialised_extracted_accounts_waiting_for_approval = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseApprovalModel>>(extracted_accounts_waiting_for_approval);
                        for (int i = 0; i < deserialised_extracted_accounts_waiting_for_approval?.Keys.Count(); i++)
                            await client.Child("Accounts_Waiting_For_Approval").Child(deserialised_extracted_accounts_waiting_for_approval.Keys.ElementAt(i)).DeleteAsync();


                        string? extracted_accounts_waiting_for_deletion = await client.Child("Accounts_Waiting_For_Deletion").OrderBy("email").EqualTo(base64_email).OnceAsJsonAsync();
                        Dictionary<string, FirebaseApprovalModel>? deserialised_extracted_accounts_waiting_for_deletion = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseApprovalModel>>(extracted_accounts_waiting_for_deletion);


                        for (int i = 0; i < deserialised_extracted_accounts_waiting_for_deletion?.Keys.Count(); i++)
                            await client.Child("Accounts_Waiting_For_Deletion").Child(deserialised_extracted_accounts_waiting_for_deletion.Keys.ElementAt(i)).DeleteAsync();


                        string? extracted_accounts_waiting_for_password_change = await client.Child("Accounts_Waiting_For_Password_Change").OrderBy("email").EqualTo(base64_email).OnceAsJsonAsync();
                        Dictionary<string, FirebaseApprovalModel>? deserialised_extracted_accounts_waiting_for_password_change = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseApprovalModel>>(extracted_accounts_waiting_for_password_change);


                        for (int i = 0; i < deserialised_extracted_accounts_waiting_for_password_change?.Keys.Count(); i++)
                            await client.Child("Accounts_Waiting_For_Password_Change").Child(deserialised_extracted_accounts_waiting_for_password_change.Keys.ElementAt(i)).DeleteAsync();

                        response = "Account deletion successful";
                    }
                    else
                    {
                        response = "Account does not exists";
                    }
                }
                else
                {
                    response = "Internal server error";
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Account deletion validation API error");
                response = "Internal server error";
            }

            return response;
        }


        public async Task<string> ValidateAccountUpdate(PasswordUpdateValidationModel? value)
        {
            string? response = "Internal server error";

            try
            {
                if (value != null)
                {
                    if (value.code != null)
                    {
                        if (value.new_password != null)
                        {
                            FirebaseClient? client = await Shared.firebase.Firebase();

                            if (client != null)
                            {
                                Tuple<string, Type> hashed_code = await Sha512Hasher.Hash(value.code);

                                if (hashed_code.Item2 != typeof(Exception))
                                {
                                    string base64_hashed_code = await Base64Formatter.FromUtf8ToBase64(hashed_code.Item1);

                                    string extracted_update_session = await client.Child("Accounts_Waiting_For_Password_Change").OrderBy("key").EqualTo(base64_hashed_code).OnceAsJsonAsync();
                                    Dictionary<string, FirebaseApprovalModel>? deserialised_update_session = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseApprovalModel>?>(extracted_update_session);

                                    if (deserialised_update_session?.Keys.Count() == 1)
                                    {
                                        FirebaseApprovalModel model = deserialised_update_session.Values.ElementAt(0);

                                        if (model.expiry_date > Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm")))
                                        {
                                            await client.Child("Accounts_Waiting_For_Password_Change").Child(deserialised_update_session.Keys.ElementAt(0)).DeleteAsync();

                                            string extracted_credentials = await client.Child("Credentials").OrderBy("email").EqualTo(model.email).OnceAsJsonAsync();
                                            Dictionary<string, FirebaseCredentialModel>? deserialised_credentials = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseCredentialModel>?>(extracted_credentials);

                                            if (deserialised_credentials?.Keys.Count() == 1)
                                            {
                                                string base64_password = await Base64Formatter.FromUtf8ToBase64(value.new_password);
                                                FirebaseCredentialModel credentials = deserialised_credentials.Values.ElementAt(0);
                                                credentials.password = base64_password;

                                                await client.Child("Credentials").Child(deserialised_credentials?.Keys.ElementAt(0)).PutAsync(credentials);

                                                response = "Password update successful";
                                            }
                                            else
                                            {
                                                response = "Account does not exist";
                                            }
                                        }
                                        else
                                        {
                                            response = "Password update code expired";
                                        }
                                    }
                                    else
                                    {
                                        response = "Invalid code";
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
                            response = "Invalid password";
                        }
                    }
                    else
                    {
                        response = "Invalid code";
                    }
                }
                else
                {
                    response = "Internal server error";
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Password update validation API error");
                response = "Internal server error";
            }

            return response;
        }
    }
}
