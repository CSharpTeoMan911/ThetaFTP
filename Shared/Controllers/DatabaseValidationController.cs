using ThetaFTP.Shared.Formatters;
using MySql.Data.MySqlClient;
using Serilog;
using System.Data.Common;
using System.Text;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;
using ThetaFTP.Pages.Components;

namespace ThetaFTP.Shared.Controllers
{
    public class DatabaseValidationController 
    {
        public async Task<PayloadModel?> ValidateAccount(ValidationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (value?.email != null)
            {
                if (value?.code != null)
                {
                    if (Shared.sha512 != null)
                    {
                        string hashed_key = await Shared.sha512.Hash(value.code);

                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                        try
                        {
                            MySqlCommand validation_command = connection.CreateCommand();
                            try
                            {
                                validation_command.CommandText = "SELECT Email FROM accounts_waiting_for_approval WHERE Account_Validation_Code = @Account_Validation_Code";
                                validation_command.Parameters.AddWithValue("Account_Validation_Code", hashed_key);

                                DbDataReader reader = await validation_command.ExecuteReaderAsync();
                                try
                                {
                                    if (await reader.ReadAsync() == true)
                                    {
                                        string? extracted_value = reader.GetString(0);
                                        await reader.CloseAsync();

                                        if (extracted_value == value?.email)
                                        {
                                            MySqlCommand approval_command = connection.CreateCommand();
                                            try
                                            {
                                                approval_command.CommandText = "DELETE FROM accounts_waiting_for_approval WHERE Account_Validation_Code = @Account_Validation_Code";
                                                approval_command.Parameters.AddWithValue("Account_Validation_Code", hashed_key);
                                                await approval_command.ExecuteNonQueryAsync();

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

                                                    payloadModel.result = "Account authorised";
                                                    payloadModel.payload = log_in_session_key;
                                                    payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.Error(e, "Account validation API error");
                                                    payloadModel.result = "Internal server error";
                                                }
                                                finally
                                                {
                                                    await insert_log_in_key_command.DisposeAsync();
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Log.Error(e, "Account validation API error");
                                                payloadModel.result = "Internal server error";
                                            }
                                            finally
                                            {
                                                await approval_command.DisposeAsync();
                                            }
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
                                catch (Exception e)
                                {
                                    Log.Error(e, "Account validation API error");
                                    payloadModel.result = "Internal server error";
                                }
                                finally
                                {
                                    await reader.DisposeAsync();
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Account validation API error");
                                payloadModel.result = "Internal server error";
                            }
                            finally
                            {
                                await validation_command.DisposeAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Account validation API error");
                            payloadModel.result = "Internal server error";
                        }
                        finally
                        {
                            await connection.DisposeAsync();
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

            return payloadModel;
        }

        public async Task<PayloadModel?> ValidateLogInSession(ValidationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();


            if (value?.email != null)
            {
                if (value?.code != null)
                {
                    if (Shared.sha512 != null)
                    {
                        string hashed_key = await Shared.sha512.Hash(value.code);
                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                        try
                        {
                            MySqlCommand validation_command = connection.CreateCommand();
                            try
                            {
                                validation_command.CommandText = "SELECT Log_In_Code FROM log_in_sessions_waiting_for_approval WHERE Log_In_Code = @Log_In_Code";
                                validation_command.Parameters.AddWithValue("Log_In_Code", hashed_key);

                                DbDataReader reader = await validation_command.ExecuteReaderAsync();
                                try
                                {
                                    if (await reader.ReadAsync() == true)
                                    {
                                        await reader.CloseAsync();
                                        MySqlCommand approval_command = connection.CreateCommand();
                                        try
                                        {
                                            approval_command.CommandText = "DELETE FROM log_in_sessions_waiting_for_approval WHERE Log_In_Code = @Log_In_Code";
                                            approval_command.Parameters.AddWithValue("Log_In_Code", hashed_key);
                                            await approval_command.ExecuteNonQueryAsync();

                                            payloadModel.result = "Authentication successful";
                                            payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error(e, "Log in session validation API error");
                                            payloadModel.result = "Internal server error";
                                        }
                                        finally
                                        {
                                            await approval_command.DisposeAsync();
                                        }
                                    }
                                    else
                                    {
                                        payloadModel.result = "Invalid code";
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e, "Log in session validation API error");
                                    payloadModel.result = "Internal server error";
                                }
                                finally
                                {
                                    await reader.DisposeAsync();
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Log in session validation API error");
                                payloadModel.result = "Internal server error";
                            }
                            finally
                            {
                                await validation_command.DisposeAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Log in session validation API error");
                            payloadModel.result = "Internal server error";
                        }
                        finally
                        {
                            await connection.DisposeAsync();
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
            return payloadModel;
        }

        public async Task<PayloadModel?> ValidateLogInSessionKey(string? code)
        {
            PayloadModel? payloadModel = new PayloadModel();
            if (code != null)
            {
                if (Shared.sha512 != null)
                {
                    string hashed_key = await Shared.sha512.Hash(code);
                    MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                    try
                    {
                        MySqlCommand log_in_session_key_validation = connection.CreateCommand();
                        try
                        {
                            log_in_session_key_validation.CommandText = "SELECT Expiration_Date, Email FROM log_in_sessions WHERE Log_In_Session_Key = @Log_In_Session_Key";
                            log_in_session_key_validation.Parameters.AddWithValue("Log_In_Session_Key", hashed_key);

                            DbDataReader log_in_session_key_validation_reader = await log_in_session_key_validation.ExecuteReaderAsync();
                            try
                            {
                                if (await log_in_session_key_validation_reader.ReadAsync() == true)
                                {
                                    DateTime expiration_date = (DateTime)log_in_session_key_validation_reader.GetValue(0);
                                    string email = log_in_session_key_validation_reader.GetString(1);

                                    await log_in_session_key_validation_reader.CloseAsync();

                                    if (DateTime.Now < expiration_date)
                                    {
                                        if (Shared.configurations?.twoStepAuth == true)
                                        {

                                            MySqlCommand log_in_session_key_is_validated = connection.CreateCommand();
                                            log_in_session_key_is_validated.CommandText = "SELECT Log_In_Code FROM log_in_sessions_waiting_for_approval WHERE Log_In_Session_Key = @Log_In_Session_Key";
                                            log_in_session_key_is_validated.Parameters.AddWithValue("Log_In_Session_Key", hashed_key);

                                            try
                                            {
                                                DbDataReader log_in_session_key_is_validated_reader = await log_in_session_key_is_validated.ExecuteReaderAsync();
                                                try
                                                {
                                                    if (await log_in_session_key_is_validated_reader.ReadAsync() == false)
                                                    {
                                                        payloadModel.payload = email;
                                                        payloadModel.result = "Log in session approved";
                                                        payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                                    }
                                                    else
                                                    {
                                                        payloadModel.result = "Log in session not approved";
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Log.Error(e, "Log in session key validation API error");
                                                    payloadModel.result = "Internal server error";
                                                }
                                                finally
                                                {
                                                    await log_in_session_key_is_validated_reader.DisposeAsync();
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                Log.Error(e, "Log in session key validation API error");
                                                payloadModel.result = "Internal server error";
                                            }
                                            finally
                                            {
                                                await log_in_session_key_is_validated.DisposeAsync();
                                            }
                                        }
                                        else
                                        {
                                            payloadModel.payload = email;
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
                            catch (Exception e)
                            {
                                Log.Error(e, "Log in session key validation API error");
                                payloadModel.result = "Internal server error";
                            }
                            finally
                            {
                                await log_in_session_key_validation_reader.DisposeAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Log in session key validation API error");
                            payloadModel.result = "Internal server error";
                        }
                        finally
                        {
                            await log_in_session_key_validation.DisposeAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Log in session key validation API error");
                        payloadModel.result = "Internal server error";
                    }
                    finally
                    {
                        await connection.DisposeAsync();
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

            return payloadModel;
        }

        public async Task<PayloadModel?> DeleteLogInSession(string? code)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (code != null)
            {
                if (Shared.sha512 != null)
                {
                    string hashed_key = await Shared.sha512.Hash(code);
                    MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                    try
                    {
                        MySqlCommand delete_key_command = connection.CreateCommand();
                        try
                        {
                            delete_key_command.CommandText = "DELETE FROM log_in_sessions WHERE Log_In_Session_Key = @Log_In_Session_Key";
                            delete_key_command.Parameters.AddWithValue("Log_In_Session_Key", hashed_key);
                            await delete_key_command.ExecuteNonQueryAsync();

                            payloadModel.result = "Log out successful";
                            payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Account log out API error");
                            payloadModel.result = "Internal server error";
                        }
                        finally
                        {
                            await delete_key_command.DisposeAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Account log out API error");
                        payloadModel.result = "Internal server error";
                    }
                    finally
                    {
                        await connection.DisposeAsync();
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

            return payloadModel;
        }

        public async Task<PayloadModel?> ValidateAccountDeletion(string? code)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (code != null)
            {
                if (Shared.sha512 != null)
                {
                    string hashed_key = await Shared.sha512.Hash(code);
                    MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                    try
                    {
                        MySqlCommand accounts_waiting_for_deletion_command = connection.CreateCommand();
                        try
                        {
                            accounts_waiting_for_deletion_command.CommandText = "SELECT Email, Expiration_Date FROM accounts_waiting_for_deletion WHERE Account_Deletion_Code = @Account_Deletion_Code";
                            accounts_waiting_for_deletion_command.Parameters.AddWithValue("Account_Deletion_Code", hashed_key);
                            DbDataReader reader = await accounts_waiting_for_deletion_command.ExecuteReaderAsync();
                            try
                            {
                                if (await reader.ReadAsync() == true)
                                {
                                    DateTime expiration_date = (DateTime)reader.GetValue(1);

                                    if (DateTime.Now < expiration_date)
                                    {
                                        string email = reader.GetString(0);

                                        await reader.CloseAsync();

                                        MySqlCommand account_deletion_command = connection.CreateCommand();
                                        try
                                        {
                                            StringBuilder builder = new StringBuilder(Environment.CurrentDirectory);
                                            builder.Append(FileSystemFormatter.PathSeparator());
                                            builder.Append("FTP_Server");
                                            builder.Append(FileSystemFormatter.PathSeparator());
                                            builder.Append(email);

                                            FileSystemFormatter.DeleteDirectory(builder.ToString());

                                            account_deletion_command.CommandText = "DELETE FROM credentials WHERE Email = @Email";
                                            account_deletion_command.Parameters.AddWithValue("Email", email);
                                            await account_deletion_command.ExecuteNonQueryAsync();

                                            payloadModel.result = "Account deletion successful";
                                            payloadModel.StatusCode = System.Net.HttpStatusCode.OK;
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error(e, "Account deletion validation API error");
                                            payloadModel.result = "Internal server error";
                                        }
                                        finally
                                        {
                                            await account_deletion_command.DisposeAsync();
                                        }
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
                            catch (Exception e)
                            {
                                Log.Error(e, "Account deletion validation API error");
                                payloadModel.result = "Internal server error";
                            }
                            finally
                            {
                                await reader.DisposeAsync();
                            }
                            
                        }
                        catch (Exception e)
                        {
                            Log.Error(e, "Account deletion validation API error");
                            payloadModel.result = "Internal server error";
                        }
                        finally
                        {
                            await accounts_waiting_for_deletion_command.DisposeAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Account deletion validation API error");
                        payloadModel.result = "Internal server error";
                    }
                    finally
                    {
                        await connection.DisposeAsync();
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

            return payloadModel;
        }

        public async Task<PayloadModel?> ValidateAccountUpdate(PasswordUpdateValidationModel? value)
        {
            PayloadModel? payloadModel = new PayloadModel();

            if (value != null)
            {
                if (value.code != null)
                {
                    if (value.new_password != null)
                    {
                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                        try
                        {
                            if (connection.State == System.Data.ConnectionState.Open)
                            {
                                if (Shared.sha512 != null)
                                {
                                    string hashed_code = await Shared.sha512.Hash(value.code);
                                    MySqlCommand account_update_session_command = connection.CreateCommand();
                                    try
                                    {
                                        account_update_session_command.CommandText = "SELECT Email, Expiration_Date FROM accounts_waiting_for_password_change WHERE Account_Password_Change_Code = @Account_Password_Change_Code";
                                        account_update_session_command.Parameters.AddWithValue("Account_Password_Change_Code", hashed_code);
                                        DbDataReader account_update_session_reader = await account_update_session_command.ExecuteReaderAsync();
                                        try
                                        {
                                            if (await account_update_session_reader.ReadAsync() == true)
                                            {
                                                string email = account_update_session_reader.GetString(0);
                                                DateTime expiration_date = (DateTime)account_update_session_reader.GetValue(1);

                                                await account_update_session_reader.CloseAsync();

                                                if (expiration_date > DateTime.Now)
                                                {
                                                    MySqlCommand delete_account_update_session_command = connection.CreateCommand();
                                                    try
                                                    {
                                                        delete_account_update_session_command.CommandText = "DELETE FROM accounts_waiting_for_password_change WHERE Account_Password_Change_Code = @Account_Password_Change_Code";
                                                        delete_account_update_session_command.Parameters.AddWithValue("Account_Password_Change_Code", hashed_code);
                                                        await delete_account_update_session_command.ExecuteNonQueryAsync();
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Log.Error(e, "Password update validation API error");
                                                        payloadModel.result = "Internal server error";
                                                    }
                                                    finally
                                                    {
                                                        await delete_account_update_session_command.DisposeAsync();
                                                    }

                                                    if (Shared.sha512 != null)
                                                    {
                                                        string hashed_password = await Shared.sha512.Hash(value.new_password);

                                                        MySqlCommand update_password_command = connection.CreateCommand();
                                                        try
                                                        {
                                                            update_password_command.CommandText = "UPDATE credentials SET Password = @Password WHERE Email = @Email";
                                                            update_password_command.Parameters.AddWithValue("Email", email);
                                                            update_password_command.Parameters.AddWithValue("Password", hashed_password);
                                                            await update_password_command.ExecuteNonQueryAsync();

                                                            payloadModel.result = "Password update successful";
                                                            payloadModel.StatusCode = System.Net.HttpStatusCode.OK;

                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Log.Error(e, "Password update validation API error");
                                                            payloadModel.result = "Internal server error";
                                                        }
                                                        finally
                                                        {
                                                            await update_password_command.DisposeAsync();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        payloadModel.result = "Internal server error";
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
                                        catch (Exception e)
                                        {
                                            Log.Error(e, "Password update validation API error");
                                            payloadModel.result = "Internal server error";
                                        }
                                        finally
                                        {
                                            await account_update_session_reader.DisposeAsync();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error(e, "Password update validation API error");
                                        payloadModel.result = "Internal server error";
                                    }
                                    finally
                                    {
                                        await account_update_session_command.DisposeAsync();
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
                        catch (Exception e)
                        {
                            Log.Error(e, "Password update validation API error");
                            payloadModel.result = "Internal server error";
                        }
                        finally
                        {
                            await connection.DisposeAsync();
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

            return payloadModel;
        }
    }
}
