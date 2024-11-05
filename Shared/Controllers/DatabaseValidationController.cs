using HallRentalSystem.Classes.StructuralAndBehavioralElements.Formaters;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Text;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    public class DatabaseValidationController 
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
                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                        try
                        {
                            MySqlCommand validation_command = connection.CreateCommand();
                            try
                            {
                                validation_command.CommandText = "SELECT Email FROM accounts_waiting_for_approval WHERE Account_Validation_Code = @Account_Validation_Code";
                                validation_command.Parameters.AddWithValue("Account_Validation_Code", hashed_key.Item1);

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
                                                approval_command.Parameters.AddWithValue("Account_Validation_Code", hashed_key.Item1);
                                                await approval_command.ExecuteNonQueryAsync();

                                                string? log_in_session_key = await CodeGenerator.GenerateKey(40);
                                                Tuple<string, Type> hashed_log_in_session_key = await Sha512Hasher.Hash(log_in_session_key);

                                                if (hashed_log_in_session_key.Item2 != typeof(Exception) && log_in_session_key != null)
                                                {
                                                    MySqlCommand insert_log_in_key_command = connection.CreateCommand();
                                                    try
                                                    {

                                                        insert_log_in_key_command.CommandText = "INSERT INTO log_in_sessions VALUES(@Log_In_Session_Key, @Email, @Expiration_Date)";
                                                        insert_log_in_key_command.Parameters.AddWithValue("Log_In_Session_Key", hashed_log_in_session_key.Item1);
                                                        insert_log_in_key_command.Parameters.AddWithValue("Email", value.email);
                                                        insert_log_in_key_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddDays(2));

                                                        await insert_log_in_key_command.ExecuteNonQueryAsync();

                                                        serverPayload.response_message = "Account authorised";
                                                        serverPayload.content = log_in_session_key;
                                                    }
                                                    finally
                                                    {
                                                        await insert_log_in_key_command.DisposeAsync();
                                                    }
                                                }
                                                else
                                                {
                                                    serverPayload.response_message = "Internal server error";
                                                }
                                            }
                                            finally
                                            {
                                                await approval_command.DisposeAsync();
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
                                finally
                                {
                                    await reader.DisposeAsync();
                                }
                            }
                            finally
                            {
                                await validation_command.DisposeAsync();
                            }
                        }
                        finally
                        {
                            await connection.DisposeAsync();
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
                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                        try
                        {
                            MySqlCommand validation_command = connection.CreateCommand();
                            try
                            {
                                validation_command.CommandText = "SELECT Log_In_Code FROM log_in_sessions_waiting_for_approval WHERE Log_In_Code = @Log_In_Code";
                                validation_command.Parameters.AddWithValue("Log_In_Code", hashed_key.Item1);

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
                                            approval_command.Parameters.AddWithValue("Log_In_Code", hashed_key.Item1);
                                            await approval_command.ExecuteNonQueryAsync();
                                            serverPayload.response_message = "Authentication successful";
                                        }
                                        finally
                                        {
                                            await approval_command.DisposeAsync();
                                        }
                                    }
                                    else
                                    {
                                        serverPayload.response_message = "Invalid code";
                                    }
                                }
                                finally
                                {
                                    await reader.DisposeAsync();
                                }
                            }
                            finally
                            {
                                await validation_command.DisposeAsync();
                            }
                        }
                        finally
                        {
                            await connection.DisposeAsync();
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
                    MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                    try
                    {
                        MySqlCommand log_in_session_key_validation = connection.CreateCommand();
                        try
                        {
                            log_in_session_key_validation.CommandText = "SELECT Expiration_Date, Email FROM log_in_sessions WHERE Log_In_Session_Key = @Log_In_Session_Key";
                            log_in_session_key_validation.Parameters.AddWithValue("Log_In_Session_Key", hashed_key.Item1);

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
                                        if (Shared.configurations?.two_step_auth == true)
                                        {

                                            MySqlCommand log_in_session_key_is_validated = connection.CreateCommand();
                                            log_in_session_key_is_validated.CommandText = "SELECT Log_In_Code FROM log_in_sessions_waiting_for_approval WHERE Log_In_Session_Key = @Log_In_Session_Key";
                                            log_in_session_key_is_validated.Parameters.AddWithValue("Log_In_Session_Key", hashed_key.Item1);

                                            try
                                            {
                                                DbDataReader log_in_session_key_is_validated_reader = await log_in_session_key_is_validated.ExecuteReaderAsync();
                                                try
                                                {
                                                    if (await log_in_session_key_is_validated_reader.ReadAsync() == false)
                                                    {
                                                        response = email;
                                                    }
                                                    else
                                                    {
                                                        response = "Log in session not approved";
                                                    }
                                                }
                                                finally
                                                {
                                                    await log_in_session_key_is_validated_reader.DisposeAsync();
                                                }
                                            }
                                            finally
                                            {
                                                await log_in_session_key_is_validated.DisposeAsync();
                                            }
                                        }
                                        else
                                        {
                                            response = email;
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
                            finally
                            {
                                await log_in_session_key_validation_reader.DisposeAsync();
                            }
                        }
                        finally
                        {
                            await log_in_session_key_validation.DisposeAsync();
                        }
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
                    MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                    try
                    {
                        MySqlCommand delete_key_command = connection.CreateCommand();
                        try
                        {
                            delete_key_command.CommandText = "DELETE FROM log_in_sessions WHERE Log_In_Session_Key = @Log_In_Session_Key";
                            delete_key_command.Parameters.AddWithValue("Log_In_Session_Key", hashed_key.Item1);
                            await delete_key_command.ExecuteNonQueryAsync();

                            response = "Log out successful";
                        }
                        finally
                        {
                            await delete_key_command.DisposeAsync();
                        }
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
            }
            else
            {
                response = "Invalid code";
            }

            return response;
        }

        public async Task<string?> ValidateAccountDeletion(string? code)
        {
            string? response = "Internal server error";

            if (code != null)
            {
                Tuple<string, Type> hashed_key = await Sha512Hasher.Hash(code);

                if (hashed_key.Item2 != typeof(Exception))
                {
                    MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                    try
                    {
                        MySqlCommand accounts_waiting_for_deletion_command = connection.CreateCommand();
                        try
                        {
                            accounts_waiting_for_deletion_command.CommandText = "SELECT Email, Expiration_Date FROM accounts_waiting_for_deletion WHERE Account_Deletion_Code = @Account_Deletion_Code";
                            accounts_waiting_for_deletion_command.Parameters.AddWithValue("Account_Deletion_Code", hashed_key.Item1);
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

                                            response = "Account deletion successful";
                                        }
                                        catch
                                        {
                                            response = "Internal server error";
                                        }
                                        finally
                                        {
                                            await account_deletion_command.DisposeAsync();
                                        }
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
                            finally
                            {
                                await reader.DisposeAsync();
                            }
                            
                        }
                        catch
                        {
                            response = "Internal server error";
                        }
                        finally
                        {
                            await accounts_waiting_for_deletion_command.DisposeAsync();
                        }
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
            }
            else
            {
                response = "Invalid code";
            }

            return response;
        }

        public async Task<string> ValidateAccountUpdate(PasswordUpdateValidationModel? value)
        {
            string? response = "Internal server error";

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
                                Tuple<string, Type> hashed_code = await Sha512Hasher.Hash(value.code);

                                if (hashed_code.Item2 != typeof(Exception))
                                {
                                    MySqlCommand account_update_session_command = connection.CreateCommand();
                                    try
                                    {
                                        account_update_session_command.CommandText = "SELECT Email, Expiration_Date FROM accounts_waiting_for_password_change WHERE Account_Password_Change_Code = @Account_Password_Change_Code";
                                        account_update_session_command.Parameters.AddWithValue("Account_Password_Change_Code", hashed_code.Item1);
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
                                                        delete_account_update_session_command.Parameters.AddWithValue("Account_Password_Change_Code", hashed_code.Item1);
                                                        await delete_account_update_session_command.ExecuteNonQueryAsync();
                                                    }
                                                    catch
                                                    {
                                                        response = "Internal server error";
                                                    }
                                                    finally
                                                    {
                                                        await delete_account_update_session_command.DisposeAsync();
                                                    }

                                                    Tuple<string, Type> hashed_password = await Sha512Hasher.Hash(value.new_password);

                                                    if (hashed_password.Item2 != typeof(Exception))
                                                    {
                                                        MySqlCommand update_password_command = connection.CreateCommand();
                                                        try
                                                        {
                                                            update_password_command.CommandText = "UPDATE credentials SET Password = @Password WHERE Email = @Email";
                                                            update_password_command.Parameters.AddWithValue("Email", email);
                                                            update_password_command.Parameters.AddWithValue("Password", hashed_password.Item1);
                                                            await update_password_command.ExecuteNonQueryAsync();

                                                            response = "Password update successful";

                                                        }
                                                        catch
                                                        {
                                                            response = "Internal server error";
                                                        }
                                                        finally
                                                        {
                                                            await update_password_command.DisposeAsync();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        response = "Internal server error";
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
                                        catch
                                        {
                                            response = "Internal server error";
                                        }
                                        finally
                                        {
                                            await account_update_session_reader.DisposeAsync();
                                        }
                                    }
                                    catch
                                    {
                                        response = "Internal server error";
                                    }
                                    finally
                                    {
                                        await account_update_session_command.DisposeAsync();
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
                        catch
                        {
                            response = "Internal server error";
                        }
                        finally
                        {
                            await connection.DisposeAsync();
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

            return response;
        }
    }
}
