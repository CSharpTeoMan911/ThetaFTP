﻿using Firebase.Database;
using Firebase.Database.Query;
using ThetaFTP.Shared.Models;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Classes;

namespace ThetaFTP.Shared.Controllers
{
    public class FirebaseDatabaseServerFunctionsController
    {
        public async Task DeleteDatabaseCache()
        {
            await DeleteExpiredAccountsWaitingForApproval();
            await DeleteAccountsWaitingForDeletion();
            await DeleteAccountsWaitingForPasswordChange();
            await DeleteLogInSessionWaitingForApproval();
            await DeleteLogInSession();
        }

        private async Task DeleteExpiredAccountsWaitingForApproval()
        {
            FirebaseClient? client = await Shared.firebase.Firebase();
            try
            {
                if (client != null)
                {
                    string? extracted_cache = await client.Child("Accounts_Waiting_For_Approval").OrderBy("expiry_date").EndAt(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm"))).LimitToFirst(100).OnceAsJsonAsync();
                    Dictionary<string, FirebaseApprovalModel>? deserialised_extracted_cache = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseApprovalModel>?>(extracted_cache);

                    for (int i = 0; i < deserialised_extracted_cache?.Count(); i++)
                        await client.Child("Accounts_Waiting_For_Approval").Child(deserialised_extracted_cache.Keys.ElementAt(i)).DeleteAsync();
                }
            }
            catch(Exception e)
            {
                Logging.Message(e, "Error deleting expired accounts", "Error deleting expired accounts", "FirebaseDatabaseServerFunctionsController", "DeleteExpiredAccountsWaitingForApproval", Logging.LogType.Error);
            }
            finally
            {
                client?.Dispose();
            }
        }

        private async Task DeleteAccountsWaitingForDeletion()
        {
            FirebaseClient? client = await Shared.firebase.Firebase();
            try
            {
                if (client != null)
                {
                    string? extracted_cache = await client.Child("Accounts_Waiting_For_Deletion").OrderBy("expiry_date").EndAt(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm"))).LimitToFirst(100).OnceAsJsonAsync();
                    Dictionary<string, FirebaseApprovalModel>? deserialised_extracted_cache = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseApprovalModel>?>(extracted_cache);

                    for (int i = 0; i < deserialised_extracted_cache?.Count(); i++)
                        await client.Child("Accounts_Waiting_For_Deletion").Child(deserialised_extracted_cache.Keys.ElementAt(i)).DeleteAsync();
                }
            }
            catch (Exception e)
            {
                Logging.Message(e, "Error deleting accounts deletion requests", "Error deleting accounts deletion requests", "FirebaseDatabaseServerFunctionsController", "DeleteAccountsWaitingForDeletion", Logging.LogType.Error);
            }
            finally
            {
                client?.Dispose();
            }
        }

        private async Task DeleteAccountsWaitingForPasswordChange()
        {
            FirebaseClient? client = await Shared.firebase.Firebase();
            try
            {
                if (client != null)
                {
                    string? extracted_cache = await client.Child("Accounts_Waiting_For_Password_Change").OrderBy("expiry_date").EndAt(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm"))).LimitToFirst(100).OnceAsJsonAsync();
                    Dictionary<string, FirebaseApprovalModel>? deserialised_extracted_cache = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseApprovalModel>?>(extracted_cache);

                    for (int i = 0; i < deserialised_extracted_cache?.Count(); i++)
                        await client.Child("Accounts_Waiting_For_Password_Change").Child(deserialised_extracted_cache.Keys.ElementAt(i)).DeleteAsync();
                }
            }
            catch (Exception e)
            {
                Logging.Message(e, "Error deleting password update requests", "Error deleting password update requests", "FirebaseDatabaseServerFunctionsController", "DeleteAccountsWaitingForPasswordChange", Logging.LogType.Error);
            }
            finally
            {
                client?.Dispose();
            }
        }

        private async Task DeleteLogInSessionWaitingForApproval()
        {
            FirebaseClient? client = await Shared.firebase.Firebase();
            try
            {
                if (client != null)
                {
                    string? extracted_cache = await client.Child("Log_In_Sessions_Waiting_For_Approval").OrderBy("expiry_date").EndAt(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm"))).LimitToFirst(100).OnceAsJsonAsync();
                    Dictionary<string, FirebaseLogInSessionApprovalModel>? deserialised_extracted_cache = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseLogInSessionApprovalModel>?>(extracted_cache);

                    for (int i = 0; i < deserialised_extracted_cache?.Count(); i++)
                    {
                        FirebaseLogInSessionApprovalModel session = deserialised_extracted_cache.Values.ElementAt(i);
                        await client.Child("Log_In_Sessions_Waiting_For_Approval").Child(deserialised_extracted_cache.Keys.ElementAt(i)).DeleteAsync();
                        
                        string extracted_login_cache = await client.Child("Log_In_Sessions").OrderBy("key").EqualTo(session.key).OnceAsJsonAsync();
                        Dictionary<string, FirebaseLogInSessionModel>? deserialised_login_cache = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseLogInSessionModel>?>(extracted_login_cache);

                        if (deserialised_login_cache?.Keys.Count() == 1)
                        {
                            await client.Child("Log_In_Sessions").Child(deserialised_login_cache.Keys.ElementAt(0)).DeleteAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.Message(e, "Error deleting log in requests", "Error deleting log in requests", "FirebaseDatabaseServerFunctionsController", "DeleteLogInSessionWaitingForApproval", Logging.LogType.Error);
            }
            finally
            {
                client?.Dispose();
            }
        }

        private async Task DeleteLogInSession()
        {
            FirebaseClient? client = await Shared.firebase.Firebase();
            try
            {
                if (client != null)
                {
                    string? extracted_cache = await client.Child("Log_In_Sessions").OrderBy("expiry_date").EndAt(Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm"))).LimitToFirst(100).OnceAsJsonAsync();
                    Dictionary<string, FirebaseApprovalModel>? deserialised_extracted_cache = await JsonFormatter.JsonDeserialiser<Dictionary<string, FirebaseApprovalModel>?>(extracted_cache);

                    for (int i = 0; i < deserialised_extracted_cache?.Count(); i++)
                        await client.Child("Log_In_Sessions").Child(deserialised_extracted_cache.Keys.ElementAt(i)).DeleteAsync();
                }
            }
            catch (Exception e)
            {
                Logging.Message(e, "Error deleting log in sessions", "Error deleting log in sessions", "FirebaseDatabaseServerFunctionsController", "DeleteLogInSession", Logging.LogType.Error);
            }
            finally
            {
                client?.Dispose();
            }
        }
    }
}
