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
    }
}
