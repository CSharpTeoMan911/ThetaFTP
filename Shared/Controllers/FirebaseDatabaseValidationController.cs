using Firebase.Database;
using Firebase.Database.Query;
using ThetaFTP.Shared.Formatters;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Serilog;
using System.Data.Common;
using System.Text;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;
using ThetaFTP.Pages.Components;

namespace ThetaFTP.Shared.Controllers
{
    public class FirebaseDatabaseValidationController
    {
        public async Task<PayloadModel?> ValidateAccount(ValidationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            try
            {
                if (value?.email != null)
                {
                    if (value?.code != null)
                    {
                        if (Shared.sha512 != null)
                        {
                            string hashed_key = await Shared.sha512.Hash(value.code);

                            string base64_email = await Base64Formatter.FromUtf8ToBase64(value.email);
                            string base64_hashed_key = await Base64Formatter.FromUtf8ToBase64(hashed_key);

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
                                        string hashed_log_in_session_key = await Shared.sha512.Hash(log_in_session_key);

                                        string base64_hashed_log_in_session_key = await Base64Formatter.FromUtf8ToBase64(hashed_log_in_session_key);

                                        FirebaseLogInSessionModel firebaseLogInSessionModel = new FirebaseLogInSessionModel()
                                        {
                                            email = base64_email,
                                            key = base64_hashed_log_in_session_key,
                                            expiry_date = Convert.ToInt64(DateTime.Now.AddDays(2).ToString("yyyyMMddHHmm"))
                                        };

                                        await client.Child("Log_In_Sessions").PostAsync(firebaseLogInSessionModel, false);

                                        payloadModel.result = "Account authorised";
                                        payloadModel.payload= log_in_session_key;
                                        payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                    }
                                    else
                                    {
                                        payloadModel.result = "Invalid code";
                                    }
                                }
                                else
                                {
                                    payloadModel.result = "Invalid code";
                                }
                            }
                            else
                            {
                                payloadModel.result = "Internal server error";
                            }
                        }
                        else
                        {
                            payloadModel.result = "Internal server error";
                        }
                    }
                    else
                    {
                        payloadModel.result = "Invalid code";
                    }
                }
                else
                {
                    payloadModel.result = "Invalid email";
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Account validation API error");
                payloadModel.result = "Internal server error";
            }

            return payloadModel;
        }

        public async Task<PayloadModel?> ValidateLogInSession(ValidationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            try
            {
                if (value?.email != null)
                {
                    if (value?.code != null)
                    {
                        if (Shared.sha512 != null)
                        {
                            string hashed_key = await Shared.sha512.Hash(value.code);
                            string base64_hashed_key = await Base64Formatter.FromUtf8ToBase64(hashed_key);

                            FirebaseClient? client = await Shared.firebase.Firebase();

                            if (client != null)
                            {
                                string extracted_log_in_session = await client.Child("Log_In_Sessions_Waiting_For_Approval").OrderBy("code").EqualTo(base64_hashed_key).OnceAsJsonAsync();
                                Dictionary<string, FirebaseLogInSessionModel>? deserialised_log_in_session = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseLogInSessionModel>?>(extracted_log_in_session);

                                if (deserialised_log_in_session?.Keys.Count() > 0)
                                {
                                    await client.Child("Log_In_Sessions_Waiting_For_Approval").Child(deserialised_log_in_session?.Keys.ElementAt(0)).DeleteAsync();
                                    payloadModel.result = "Authentication successful";
                                    payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                }
                                else
                                {
                                    payloadModel.result = "Invalid code";
                                }
                            }
                            else
                            {
                                payloadModel.result = "Internal server error";
                            }
                        }
                        else
                        {
                            payloadModel.result = "Internal server error";
                        }
                    }
                    else
                    {
                        payloadModel.result = "Invalid code";
                    }
                }
                else
                {
                    payloadModel.result = "Invalid email";
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Log in session validation API error");
                payloadModel.result = "Internal server error";
            }

            return payloadModel;
        }

        public async Task<PayloadModel?> ValidateLogInSessionKey(string? code)
        {
            PayloadModel? payloadModel = new PayloadModel();

            try
            {
                if (code != null)
                {
                    if (Shared.sha512 != null)
                    {
                        string hashed_key = await Shared.sha512.Hash(code);
                        string base64_hashed_key = await Base64Formatter.FromUtf8ToBase64(hashed_key);

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
                                    if (Shared.configurations?.twoStepAuth == true)
                                    {
                                        string extracted_log_in_session_waiting_for_approval = await client.Child("Log_In_Sessions_Waiting_For_Approval").OrderBy("key").EqualTo(base64_hashed_key).OnceAsJsonAsync();
                                        Dictionary<string, FirebaseLogInSessionApprovalModel>? deserialised_log_in_session_waiting_for_approval = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseLogInSessionApprovalModel>?>(extracted_log_in_session_waiting_for_approval);

                                        if (deserialised_log_in_session_waiting_for_approval?.Keys.Count() == 0)
                                        {
                                            payloadModel.payload = utf8_email;
                                            payloadModel.result = "Log in session approved";
                                            payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                        }
                                        else
                                        {
                                            payloadModel.payload = "Log in session not approved";
                                        }
                                    }
                                    else
                                    {
                                        payloadModel.payload = utf8_email;
                                        payloadModel.result = "Log in session approved";
                                        payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                    }
                                }
                                else
                                {
                                    payloadModel.result = "Log in session key expired";
                                }
                            }
                            else
                            {
                                payloadModel.result = "Invalid log in session key";
                            }

                        }
                        else
                        {
                            payloadModel.result = "Internal server error";
                        }
                    }
                    else
                    {
                        payloadModel.result = "Internal server error";
                    }
                }
                else
                {
                    payloadModel.result = "Invalid log in session key";
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Log in session key validation API error");
                payloadModel.result = "Internal server error";
            }

            return payloadModel;
        }

        public async Task<PayloadModel?> DeleteLogInSession(string? code)
        {
            PayloadModel? payloadModel = new PayloadModel();

            try
            {
                if (code != null)
                {
                    if (Shared.sha512 != null)
                    {
                        string hashed_key = await Shared.sha512.Hash(code);
                        string base64_hashed_key = await Base64Formatter.FromUtf8ToBase64(hashed_key);

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
                                    payloadModel.result = "Log out successful";
                                    payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                }
                                else
                                {
                                    payloadModel.result = "Invalid code";
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            payloadModel.result = "Internal server error";
                        }
                    }
                    else
                    {
                        payloadModel.result = "Internal server error";
                    }
                }
                else
                {
                    payloadModel.result = "Invalid code";
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Account log out API error");
                payloadModel.result = "Internal server error";
            }

            return payloadModel;
        }


        public async Task<PayloadModel?> ValidateAccountDeletion(string? code)
        {
            PayloadModel? payloadModel = new PayloadModel();

            try
            {
                if (code != null)
                {
                    if (Shared.sha512 != null)
                    {
                        string hashed_key = await Shared.sha512.Hash(code);
                        string base64_hashed_key = await Base64Formatter.FromUtf8ToBase64(hashed_key);

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
                                        return await DeleteAccount(account?.email);
                                    }
                                    else
                                    {
                                        payloadModel.result = "Account deletion code expired";
                                    }
                                }
                                else
                                {
                                    payloadModel.result = "Invalid code";
                                }
                            }
                            catch
                            {
                                payloadModel.result = "Internal server error";
                            }
                        }
                        else
                        {
                            payloadModel.result = "Internal server error";
                        }
                    }
                    else
                    {
                        payloadModel.result = "Internal server error";
                    }
                }
                else
                {
                    payloadModel.result = "Invalid code";
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Account deletion validation API error");
                payloadModel.result = "Internal server error";
            }

            return payloadModel;
        }


        private async Task<PayloadModel?> DeleteAccount(string? base64_email)
        {
            PayloadModel? payloadModel = new PayloadModel();

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

                        payloadModel.result = "Account deletion successful";
                        payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                    }
                    else
                    {
                        payloadModel.result = "Account does not exists";
                    }
                }
                else
                {
                    payloadModel.result = "Internal server error";
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Account deletion validation API error");
                payloadModel.result = "Internal server error";
            }

            return payloadModel;
        }


        public async Task<PayloadModel?> ValidateAccountUpdate(PasswordUpdateValidationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

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
                                if (Shared.sha512 != null)
                                {
                                    string hashed_code = await Shared.sha512.Hash(value.code);
                                    string base64_hashed_code = await Base64Formatter.FromUtf8ToBase64(hashed_code);

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
                                                string hashed_password = await Shared.sha512.Hash(value.new_password);
                                                string base64_password = await Base64Formatter.FromUtf8ToBase64(hashed_password);

                                                FirebaseCredentialModel credentials = deserialised_credentials.Values.ElementAt(0);
                                                credentials.password = base64_password;

                                                await client.Child("Credentials").Child(deserialised_credentials?.Keys.ElementAt(0)).PutAsync(credentials);

                                                payloadModel.result = "Password update successful";
                                                payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                            }
                                            else
                                            {
                                                payloadModel.result = "Account does not exist";
                                            }
                                        }
                                        else
                                        {
                                            payloadModel.result = "Password update code expired";
                                        }
                                    }
                                    else
                                    {
                                        payloadModel.result = "Invalid code";
                                    }
                                }
                                else
                                {
                                    payloadModel.result = "Internal server error";
                                }
                            }
                            else
                            {
                                payloadModel.result = "Internal server error";
                            }
                        }
                        else
                        {
                            payloadModel.result = "Invalid password";
                        }
                    }
                    else
                    {
                        payloadModel.result = "Invalid code";
                    }
                }
                else
                {
                    payloadModel.result = "Internal server error";
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Password update validation API error");
                payloadModel.result = "Internal server error";
            }

            return payloadModel;
        }
    }
}
