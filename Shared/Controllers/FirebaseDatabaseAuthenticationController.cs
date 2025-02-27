using Firebase.Database;
using Firebase.Database.Query;
using ThetaFTP.Shared.Formatters;
using System.Text;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;
using Serilog;

namespace ThetaFTP.Shared.Controllers
{
    public class FirebaseDatabaseAuthenticationController : CRUD_Interface<AuthenticationModel, string, AuthenticationModel, PasswordUpdateModel, string, string>
    {

        public async Task<string?> Delete(string? value)
        {
            string? response = "Internal server error";

            if (value != null)
            {
                if (Shared.configurations?.twoStepAuth == true)
                {
                    FirebaseClient? client = await Shared.firebase.Firebase();

                    if (client != null)
                    {
                        try
                        {
                            string base64_email = await Base64Formatter.FromUtf8ToBase64(value);

                            string? key = await CodeGenerator.GenerateKey(10);

                            if (Shared.sha512 != null)
                            {
                                string hashed_key = await Shared.sha512.Hash(key);
                                string base64_key = await Base64Formatter.FromUtf8ToBase64(hashed_key);

                                FirebaseApprovalModel approvalModel = new FirebaseApprovalModel()
                                {
                                    email = base64_email,
                                    expiry_date = Convert.ToInt64(DateTime.Now.AddMinutes(2).ToString("yyyyMMddHHmm")),
                                    key = base64_key
                                };

                                bool smtps_operation_result = SMTPS_Service.SendSMTPS(value, "Account deletion", $"Account deletion key: {key}");

                                if (smtps_operation_result == true)
                                {
                                    await client.Child("Accounts_Waiting_For_Deletion").PostAsync(approvalModel, false);
                                    response = "Check the code sent to your email to approve the account deletion";
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
                        catch (Exception e)
                        {
                            Log.Error(e, "Account deletion API error");
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
                    response = await DeleteAccount(value);
                }
            }
            else
            {
                response = "Internal server error";
            }

            return response;
        }

        private async Task<string> DeleteAccount(string? email)
        {
            string? response = "Internal server error";

            try
            {
                FirebaseClient? client = await Shared.firebase.Firebase();

                if (client != null)
                {
                    string base64_email = await Base64Formatter.FromUtf8ToBase64(email);

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
                Log.Error(e, "Account deletion API error");
                response = "Internal server error";
            }

            return response;
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

                        try
                        {
                            if (client != null)
                            {
                                string base64_email = await Base64Formatter.FromUtf8ToBase64(value.email);

                                string extracted_credentials = await client.Child("Credentials").OrderBy("email").EqualTo(base64_email).OnceAsJsonAsync();
                                Dictionary<string, FirebaseCredentialModel>? deserialised_credentials = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseCredentialModel>?>(extracted_credentials);

                                if (deserialised_credentials?.Keys.Count() > 0)
                                {
                                    if (Shared.sha512 != null)
                                    {
                                        string hashed_password = await Shared.sha512.Hash(value?.password);
                                        string base64_hashed_password = await Base64Formatter.FromUtf8ToBase64(hashed_password);

                                        if (deserialised_credentials.Values.ElementAt(0).password == base64_hashed_password)
                                        {
                                            if (Shared.configurations?.twoStepAuth == true)
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

                                    if (Shared.sha512 != null)
                                    {
                                        string? log_in_session_key = await CodeGenerator.GenerateKey(40);
                                        string hashed_log_in_session_key = await Shared.sha512.Hash(log_in_session_key);

                                        string base64_hashed_log_in_session_key = await Base64Formatter.FromUtf8ToBase64(hashed_log_in_session_key);


                                        FirebaseLogInSessionModel firebaseLogInSessionModel = new FirebaseLogInSessionModel()
                                        {
                                            email = base64_email,
                                            expiry_date = Convert.ToInt64(DateTime.Now.AddDays(2).ToString("yyyyMMddHHmm")),
                                            key = base64_hashed_log_in_session_key
                                        };
                                        FirebaseObject<FirebaseLogInSessionModel> firebaseLogInSessionResult = await client.Child("Log_In_Sessions").PostAsync(firebaseLogInSessionModel, false);

                                        if (Shared.configurations?.twoStepAuth == true)
                                        {
                                            string? log_in_code = await CodeGenerator.GenerateKey(10);
                                            string hashed_log_in_code = await Shared.sha512.Hash(log_in_code);
                                            string base64_hashed_log_in_code = await Base64Formatter.FromUtf8ToBase64(hashed_log_in_code);
                                           
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
                        catch (Exception e)
                        {
                            Log.Error(e, "User log in API error");
                            response = "Internal server error";
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
                        if (value?.password.Length >= 10)
                        {
                            if (value?.password.Length <= 100)
                            {
                                FirebaseClient? client = await Shared.firebase.Firebase();

                                try
                                {
                                    if (client != null)
                                    {
                                        string base64_email = await Base64Formatter.FromUtf8ToBase64(value.email);

                                        string? exctracted_credentials = await client.Child("Credentials").OrderBy("email").EqualTo(base64_email).OnceAsJsonAsync();
                                        Dictionary<string, FirebaseCredentialModel>? deserialised_credentials = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseCredentialModel>>(exctracted_credentials);
                                        if (deserialised_credentials?.Keys.Count == 0)
                                        {

                                            if (Shared.sha512 != null)
                                            {
                                                string hashed_password = await Shared.sha512.Hash(value.password);
                                                string base64_hashed_password = await Base64Formatter.FromUtf8ToBase64(hashed_password);

                                                FirebaseCredentialModel credentialModel = new FirebaseCredentialModel()
                                                {
                                                    email = base64_email,
                                                    password = base64_hashed_password
                                                };

                                                FirebaseObject<FirebaseCredentialModel> result = await client.Child("Credentials").PostAsync(credentialModel, false);



                                                if (Shared.configurations?.twoStepAuth == true)
                                                {
                                                    string? key = await CodeGenerator.GenerateKey(10);
                                                    string key_hash_result = await Shared.sha512.Hash(key);

                                                    string base64_key_hash = await Base64Formatter.FromUtf8ToBase64(key_hash_result);

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
                                catch (Exception e)
                                {
                                    Log.Error(e, "User registration API error");
                                    response = "Internal server error";
                                }
                            }
                            else
                            {
                                response = "Password more than 100 characters";
                            }
                        }
                        else
                        {
                            response = "Password less than 100 characters";
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

        public async Task<string?> Update(PasswordUpdateModel? value)
        {
            string? response = "Internal server error";

            if (value != null)
            {
                if (value.email != null)
                {
                    if (value.new_password != null)
                    {
                        if (value?.new_password.Length >= 10)
                        {
                            if (value?.new_password.Length <= 100)
                            {
                                FirebaseClient? client = await Shared.firebase.Firebase();

                                try
                                {
                                    if (client != null)
                                    {
                                        if (Shared.sha512 != null)
                                        {
                                            string base64_email = await Base64Formatter.FromUtf8ToBase64(value.email);
                                            string hashed_key = await Shared.sha512.Hash(value.log_in_session_key);
                                            string base64_hashed_key = await Base64Formatter.FromUtf8ToBase64(hashed_key);

                                            if (Shared.configurations?.twoStepAuth == true)
                                            {
                                                string? code = await CodeGenerator.GenerateKey(10);
                                                string hashed_code = await Shared.sha512.Hash(code);

                                                string base64_code = await Base64Formatter.FromUtf8ToBase64(hashed_code);
                                                bool smtps_operation_result = SMTPS_Service.SendSMTPS(value?.email, "Password update", $"Password update key: {code}");

                                                if (smtps_operation_result == true)
                                                {
                                                    FirebaseApprovalModel model = new FirebaseApprovalModel()
                                                    {
                                                        email = base64_email,
                                                        expiry_date = Convert.ToInt64(DateTime.Now.AddMinutes(2).ToString("yyyyMMddHHmm")),
                                                        key = base64_code
                                                    };

                                                    await client.Child("Accounts_Waiting_For_Password_Change").PostAsync(model, false);

                                                    response = "Check the code sent to your email to approve the password change";
                                                }
                                                else
                                                {
                                                    response = "Internal server error";
                                                }
                                            }
                                            else
                                            {
                                                string extracted_credentials = await client.Child("Credentials").OrderBy("email").EqualTo(base64_email).OnceAsJsonAsync();
                                                Dictionary<string, FirebaseCredentialModel>? deserialised_credentials = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseCredentialModel>?>(extracted_credentials);

                                                if (deserialised_credentials?.Keys.Count() == 1)
                                                {
                                                    string hashed_password = await Shared.sha512.Hash(value.new_password);
                                                    string base64_password = await Base64Formatter.FromUtf8ToBase64(hashed_password);

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
                                catch (Exception e)
                                {
                                    Log.Error(e, "User password update API error");
                                    response = "Internal server error";
                                }
                            }
                            else
                            {
                                response = "Password more than 100 characters";
                            }
                        }
                        else
                        {
                            response = "Password less than 100 characters";
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
                response = "Internal server error";
            }

            return response;
        }
    }
}
