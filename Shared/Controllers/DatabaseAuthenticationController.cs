﻿using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;
using System.Data.Common;
using MySql.Data.MySqlClient;
using ThetaFTP.Shared.Formatters;
using System.Text;
using Serilog;

namespace ThetaFTP.Shared.Controllers
{
    public class DatabaseAuthenticationController : CRUD_Interface<AuthenticationModel, string, AuthenticationModel, PasswordUpdateModel, string, string>
    {
        public async Task<string?> Delete(string? value)
        {
            string? response = "Internal server error";

            if (value != null)
            {
                MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                try
                {
                    if (Shared.configurations?.twoStepAuth == true)
                    {
                        string? key = await CodeGenerator.GenerateKey(10);

                        if (Shared.sha512 != null)
                        {
                            string hashed_key = await Shared.sha512.Hash(key);
                            bool smtps_operation_result = SMTPS_Service.SendSMTPS(value, "Account deletion", $"Account deletion key: {key}");

                            if (smtps_operation_result == true)
                            {
                                MySqlCommand account_waiting_for_deletion_command = connection.CreateCommand();
                                try
                                {
                                    account_waiting_for_deletion_command.CommandText = "INSERT INTO accounts_waiting_for_deletion VALUES(@Account_Deletion_Code, @Email, @Expiration_Date)";
                                    account_waiting_for_deletion_command.Parameters.AddWithValue("Account_Deletion_Code", hashed_key);
                                    account_waiting_for_deletion_command.Parameters.AddWithValue("Email", value);
                                    account_waiting_for_deletion_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddMinutes(2));
                                    await account_waiting_for_deletion_command.ExecuteNonQueryAsync();

                                    response = "Check the code sent to your email to approve the account deletion";
                                }
                                catch(Exception e)
                                {
                                    Log.Error(e, "Deletion code insertion API error");
                                    response = "Internal server error";
                                }
                                finally
                                {
                                    await account_waiting_for_deletion_command.DisposeAsync();
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
                        MySqlCommand account_deletion_command = connection.CreateCommand();
                        try
                        {
                            StringBuilder builder = new StringBuilder(Environment.CurrentDirectory);
                            builder.Append(FileSystemFormatter.PathSeparator());
                            builder.Append("FTP_Server");
                            builder.Append(FileSystemFormatter.PathSeparator());
                            builder.Append(value);

                            FileSystemFormatter.DeleteDirectory(builder.ToString());

                            account_deletion_command.CommandText = "DELETE FROM credentials WHERE Email = @Email";
                            account_deletion_command.Parameters.AddWithValue("Email", value);
                            await account_deletion_command.ExecuteNonQueryAsync();

                            response = "Account deletion successful";
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "User account deletion API error");
                            response = "Internal server error";
                        }
                        finally
                        {
                            await account_deletion_command.DisposeAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Account deletion API error");
                    response = "Internal server error";
                }
                finally
                {
                    await connection.DisposeAsync();
                }
            }
            else
            {
                response = "Internal server error";
            }

            return response;
        }

        public async Task<string?> Get(AuthenticationModel? value)
        {
            string? response = String.Empty;

            ServerPayloadModel serverPayload = new ServerPayloadModel();
            serverPayload.response_message = "Internal server error";

            if (value != null)
            {
                if (value.email != null)
                {
                    if (value.password != null)
                    {

                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                        try
                        {
                            MySqlCommand check_if_account_exists_command = connection.CreateCommand();
                            try
                            {
                                check_if_account_exists_command.CommandText = "SELECT Password FROM credentials WHERE Email = @Email";
                                check_if_account_exists_command.Parameters.AddWithValue("Email", value.email);

                                DbDataReader check_if_account_exists_reader = await check_if_account_exists_command.ExecuteReaderAsync();
                                try
                                {
                                    if (await check_if_account_exists_reader.ReadAsync() == true)
                                    {
                                        if (Shared.sha512 != null)
                                        {
                                            string hashed_password = await Shared.sha512.Hash(value.password);

                                            if (check_if_account_exists_reader.GetString(0) == hashed_password)
                                            {
                                                await check_if_account_exists_reader.CloseAsync();

                                                if (Shared.configurations?.twoStepAuth == true)
                                                {
                                                    MySqlCommand check_if_account_is_approved_command = connection.CreateCommand();
                                                    try
                                                    {
                                                        check_if_account_is_approved_command.CommandText = "SELECT Account_Validation_Code FROM accounts_waiting_for_approval WHERE Email = @Email";
                                                        check_if_account_is_approved_command.Parameters.AddWithValue("Email", value.email);
                                                        DbDataReader check_if_account_is_approved_command_reader = await check_if_account_is_approved_command.ExecuteReaderAsync();
                                                        try
                                                        {
                                                            if (await check_if_account_is_approved_command_reader.ReadAsync() == true)
                                                            {
                                                                serverPayload.response_message = "Account not approved";
                                                                goto End;
                                                            }

                                                            await check_if_account_is_approved_command_reader.CloseAsync();
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Log.Error(e, "User account approval checkup API error");
                                                            response = "Internal server error";
                                                        }
                                                        finally
                                                        {
                                                            await check_if_account_is_approved_command_reader.DisposeAsync();
                                                        }
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Log.Error(e, "User account approval checkup API error");
                                                        response = "Internal server error";
                                                    }
                                                    finally
                                                    {
                                                        await check_if_account_is_approved_command.DisposeAsync();
                                                    }
                                                }


                                                string? log_in_session_key = await CodeGenerator.GenerateKey(40);
                                                string hashed_log_in_session_key = await Shared.sha512.Hash(log_in_session_key);

                                                MySqlCommand insert_log_in_key_command = connection.CreateCommand();
                                                try
                                                {
                                                    insert_log_in_key_command.CommandText = "INSERT INTO log_in_sessions VALUES(@Log_In_Session_Key, @Email, @Expiration_Date)";
                                                    insert_log_in_key_command.Parameters.AddWithValue("Log_In_Session_Key", hashed_log_in_session_key);
                                                    insert_log_in_key_command.Parameters.AddWithValue("Email", value.email);
                                                    insert_log_in_key_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddDays(2));
                                                    await insert_log_in_key_command.ExecuteNonQueryAsync();

                                                    if (Shared.configurations?.twoStepAuth == true)
                                                    {
                                                        string? log_in_code = await CodeGenerator.GenerateKey(10);
                                                        string hashed_log_in_code = await Shared.sha512.Hash(log_in_code);

                                                        MySqlCommand insert_log_in_code_command = connection.CreateCommand();
                                                        try
                                                        {
                                                            bool smtps_operation_result = SMTPS_Service.SendSMTPS(value?.email, "Log in authorisation", $"Login code: {log_in_code}");
                                                            insert_log_in_code_command.CommandText = "INSERT INTO log_in_sessions_waiting_for_approval VALUES(@Log_In_Code, @Log_In_Session_Key, @Expiration_Date)";
                                                            insert_log_in_code_command.Parameters.AddWithValue("Log_In_Code", hashed_log_in_code);
                                                            insert_log_in_code_command.Parameters.AddWithValue("Log_In_Session_Key", hashed_log_in_session_key);
                                                            insert_log_in_code_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddDays(2));
                                                            await insert_log_in_code_command.ExecuteNonQueryAsync();

                                                            if (smtps_operation_result == true)
                                                            {
                                                                serverPayload.content = log_in_session_key;
                                                                serverPayload.response_message = "Check the code sent to your email address to approve your log in session";
                                                            }
                                                            else
                                                            {
                                                                MySqlCommand delete_log_in_session = connection.CreateCommand();
                                                                try
                                                                {
                                                                    delete_log_in_session.CommandText = "DELETE FROM log_in_sessions WHERE Log_In_Session_Key = @Log_In_Session_Key";
                                                                    delete_log_in_session.Parameters.AddWithValue("Log_In_Session_Key", value?.email);
                                                                    await delete_log_in_session.ExecuteNonQueryAsync();
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Log.Error(e, "Log in session deletion API error");
                                                                    response = "Internal server error";
                                                                }
                                                                finally
                                                                {
                                                                    await delete_log_in_session.DisposeAsync();
                                                                }

                                                                serverPayload.response_message = "Internal server error";
                                                            }
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Log.Error(e, "Log in code insertion API error");
                                                            response = "Internal server error";
                                                        }
                                                        finally
                                                        {
                                                            await insert_log_in_code_command.DisposeAsync();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        serverPayload.content = log_in_session_key;
                                                        serverPayload.response_message = "Authentication successful";
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.Error(e, "Log in session key insertion API error");
                                                    response = "Internal server error";
                                                }
                                                finally
                                                {
                                                    await insert_log_in_key_command.DisposeAsync();
                                                }

                                            End:;
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
                                    }
                                    else
                                    {
                                        serverPayload.response_message = "Invalid email";
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e, "User account checkup API error");
                                    response = "Internal server error";
                                }
                                finally
                                {
                                    await check_if_account_exists_reader.DisposeAsync();
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "User account checkup API error");
                                response = "Internal server error";
                            }
                            finally
                            {
                                await check_if_account_exists_command.DisposeAsync();
                            }

                        }
                        catch(Exception e)
                        {
                            Log.Error(e, "User log in API error");
                            response = "Internal server error";
                        }
                        finally
                        {
                            await connection.DisposeAsync();
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
                if (value?.email != null)
                {
                    if (new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(value.email) == true)
                    {
                        if (value?.email.Length <= 100)
                        {
                            if (value?.password != null)
                            {
                                if (value?.password.Length >= 10)
                                {
                                    if (value?.password.Length <= 100)
                                    {

                                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();
                                        MySqlCommand reader_command = connection.CreateCommand();

                                        try
                                        {
                                            reader_command.CommandText = "SELECT Password FROM credentials WHERE Email = @Email";
                                            reader_command.Parameters.AddWithValue("Email", value?.email);
                                            DbDataReader reader = await reader_command.ExecuteReaderAsync();

                                            try
                                            {
                                                if (await reader.ReadAsync() == false)
                                                {
                                                    await reader.CloseAsync();
                                                    MySqlCommand credentials_insertion_command = connection.CreateCommand();

                                                    if (Shared.sha512 != null)
                                                    {
                                                        try
                                                        {
                                                            string hash_result = await Shared.sha512.Hash(value?.password);

                                                            credentials_insertion_command.CommandText = "INSERT INTO credentials VALUES(@Email, @Password)";
                                                            credentials_insertion_command.Parameters.AddWithValue("Email", value?.email);
                                                            credentials_insertion_command.Parameters.AddWithValue("Password", hash_result);
                                                            await credentials_insertion_command.ExecuteNonQueryAsync();

                                                            if (Shared.configurations?.twoStepAuth == true)
                                                            {
                                                                MySqlCommand account_validation_code_insertion_command = connection.CreateCommand();

                                                                try
                                                                {
                                                                    string? key = await CodeGenerator.GenerateKey(10);
                                                                    string key_hash_result = await Shared.sha512.Hash(key);
                                                                    bool smtps_operation_result = SMTPS_Service.SendSMTPS(value?.email, "Account approval", $"Account approval key: {key}");

                                                                    if (smtps_operation_result == true)
                                                                    {
                                                                        account_validation_code_insertion_command.CommandText = "INSERT INTO accounts_waiting_for_approval VALUES(@Account_Validation_Code, @Email, @Expiration_Date)";
                                                                        account_validation_code_insertion_command.Parameters.AddWithValue("Email", value?.email);
                                                                        account_validation_code_insertion_command.Parameters.AddWithValue("Account_Validation_Code", key_hash_result);
                                                                        account_validation_code_insertion_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddHours(1));
                                                                        await account_validation_code_insertion_command.ExecuteNonQueryAsync();

                                                                        response = "Registration successful";
                                                                    }
                                                                    else
                                                                    {
                                                                        MySqlCommand delete_account = connection.CreateCommand();
                                                                        try
                                                                        {
                                                                            delete_account.CommandText = "DELETE FROM credentials WHERE Email = @Email";
                                                                            delete_account.Parameters.AddWithValue("Email", value?.email);
                                                                            await delete_account.ExecuteNonQueryAsync();
                                                                        }
                                                                        catch (Exception e)
                                                                        {
                                                                            Log.Error(e, "Account deletion API error");
                                                                            response = "Internal server error";
                                                                        }
                                                                        finally
                                                                        {
                                                                            await delete_account.DisposeAsync();
                                                                        }

                                                                        response = "Internal server error";
                                                                    }
                                                                }
                                                                catch (Exception e)
                                                                {
                                                                    Log.Error(e, "Validation code insertion API error");
                                                                    response = "Internal server error";
                                                                }
                                                                finally
                                                                {
                                                                    await account_validation_code_insertion_command.DisposeAsync();
                                                                }
                                                            }
                                                            else
                                                            {
                                                                response = "Registration successful";
                                                            }
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Log.Error(e, "Account insertion API error");
                                                            response = "Internal server error";
                                                        }
                                                        finally
                                                        {
                                                            await credentials_insertion_command.DisposeAsync();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    response = "Account already exists";
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Log.Error(e, "Account checkup API error");
                                                response = "Internal server error";
                                            }
                                            finally
                                            {
                                                await reader.CloseAsync();
                                                await reader.DisposeAsync();
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error(e, "Registration API error");
                                            response = "Internal server error";
                                        }
                                        finally
                                        {
                                            await connection.DisposeAsync();
                                            await reader_command.DisposeAsync();
                                        }
                                    }
                                    else
                                    {
                                        response = "Password more than 100 characters";
                                    }
                                }
                                else
                                {
                                    response = "Password less than 10 characters";
                                }
                            }
                            else
                            {
                                response = "Invalid password";
                            }
                        }
                        else
                        {
                            response = "Email more than 100 characters";
                        }
                    }
                    else
                    {
                        response = "Invalid email";
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
                                MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                                try
                                {
                                    if (connection.State == System.Data.ConnectionState.Open)
                                    {
                                        if (Shared.configurations?.twoStepAuth == true)
                                        {
                                            if (Shared.sha512 != null)
                                            {
                                                string? code = await CodeGenerator.GenerateKey(10);
                                                string hashed_code = await Shared.sha512.Hash(code);

                                                bool smtps_operation_result = SMTPS_Service.SendSMTPS(value?.email, "Password update", $"Password update key: {code}");

                                                if (smtps_operation_result == true)
                                                {
                                                    MySqlCommand account_update_command = connection.CreateCommand();
                                                    try
                                                    {
                                                        account_update_command.CommandText = "INSERT INTO accounts_waiting_for_password_change VALUES(@Account_Password_Change_Code, @Email, @Expiration_Date)";
                                                        account_update_command.Parameters.AddWithValue("Account_Password_Change_Code", hashed_code);
                                                        account_update_command.Parameters.AddWithValue("Email", value?.email);
                                                        account_update_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddMinutes(2));
                                                        account_update_command.ExecuteNonQuery();

                                                        response = "Check the code sent to your email to approve the password change";
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Log.Error(e, "Password change code insertion API error");
                                                        response = "Internal server error";
                                                    }
                                                    finally
                                                    {
                                                        await account_update_command.DisposeAsync();
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
                                            if (Shared.sha512 != null)
                                            {
                                                string hashed_password = await Shared.sha512.Hash(value.new_password);

                                                MySqlCommand password_update_command = connection.CreateCommand();
                                                try
                                                {
                                                    password_update_command.CommandText = "UPDATE credentials SET Password = @Password WHERE Email = @Email";
                                                    password_update_command.Parameters.AddWithValue("Email", value.email);
                                                    password_update_command.Parameters.AddWithValue("Password", hashed_password);
                                                    await password_update_command.ExecuteNonQueryAsync();

                                                    response = "Password update successful";
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.Error(e, "Password update API error");
                                                    response = "Internal server error";
                                                }
                                                finally
                                                {
                                                    await password_update_command.DisposeAsync();
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
                                catch (Exception e)
                                {
                                    Log.Error(e, "Password update API error");
                                    response = "Internal server error";
                                }
                                finally
                                {
                                    await connection.DisposeAsync();
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
