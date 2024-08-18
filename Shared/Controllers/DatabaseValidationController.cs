using HallRentalSystem.Classes.StructuralAndBehavioralElements.Formaters;
using MySqlConnector;
using System.Data.Common;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    public class DatabaseValidationController : CRUD_Interface<string, ValidationModel, string, string>
    {
        public Task<string?> Delete(string? value)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> Get(ValidationModel? value)
        {
            string? response = String.Empty;
            ServerPayloadModel serverPayload = new ServerPayloadModel();
            serverPayload.response_message = "Internal server error";


            switch (value?.validationType)
            {
                case Shared.ValidationType.AccountAuthorisation:
                    await ValidateAccount(value, serverPayload);
                    break;
                case Shared.ValidationType.LogInSessionAuthorisation:
                    await ValidateLogInSession(value, serverPayload);
                    break;
            }

            response = await JsonFormatter.JsonSerialiser(serverPayload);
            return response;
        }

        public Task<string?> Insert(string? value)
        {
            throw new NotImplementedException();
        }

        public Task<string?> Update(string? value)
        {
            throw new NotImplementedException();
        }


        private async Task ValidateAccount(ValidationModel? value, ServerPayloadModel serverPayload)
        {
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
                                validation_command.CommandText = "SELECT Email FROM Accounts_Waiting_For_Approval WHERE Account_Validation_Code = @Account_Validation_Code";
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
                                                approval_command.CommandText = "DELETE FROM Accounts_Waiting_For_Approval WHERE Account_Validation_Code = @Account_Validation_Code";
                                                approval_command.Parameters.AddWithValue("Account_Validation_Code", hashed_key.Item1);
                                                await approval_command.ExecuteNonQueryAsync();

                                                string log_in_session_key = await CodeGenerator.GenerateKey(40);
                                                Tuple<string, Type> hashed_log_in_session_key = await Sha512Hasher.Hash(log_in_session_key);

                                                if (hashed_log_in_session_key.Item2 != typeof(Exception))
                                                {
                                                    MySqlCommand insert_log_in_key_command = connection.CreateCommand();
                                                    try
                                                    {
                                                        insert_log_in_key_command.CommandText = "INSERT INTO Log_In_Sessions VALUES(@Log_In_Session_Key, @Email, @Expiration_Date)";
                                                        insert_log_in_key_command.Parameters.AddWithValue("Log_In_Session_Key", hashed_log_in_session_key.Item1);
                                                        insert_log_in_key_command.Parameters.AddWithValue("Email", value.email);
                                                        insert_log_in_key_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddDays(2));

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
                                        serverPayload.response_message = "Internal server error";
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
        }

        private async Task ValidateLogInSession(ValidationModel? value, ServerPayloadModel serverPayload)
        {
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
                                validation_command.CommandText = "SELECT Log_In_Code FROM Log_In_Sessions_Waiting_For_Approval WHERE Log_In_Code = @Log_In_Code";
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
                                            approval_command.CommandText = "DELETE FROM Log_In_Sessions_Waiting_For_Approval WHERE Log_In_Code = @Log_In_Code";
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
        }

        private async Task ValidateLogInSessionKey(ValidationModel? value, ServerPayloadModel serverPayload)
        {
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
                            MySqlCommand log_in_session_key_validation = connection.CreateCommand();
                            try
                            {
                                log_in_session_key_validation.CommandText = "SELECT Expiration_Date FROM Log_In_Sessions WHERE Log_In_Session_Key = @Log_In_Session_Key";
                                log_in_session_key_validation.Parameters.AddWithValue("Log_In_Session_Key", hashed_key.Item1);

                                DbDataReader log_in_session_key_validation_reader = await log_in_session_key_validation.ExecuteReaderAsync();
                                try
                                {
                                    if (await log_in_session_key_validation_reader.ReadAsync() == true)
                                    {
                                        DateTime expiration_date = DateTime.Parse(log_in_session_key_validation_reader.GetString(0));

                                        if (DateTime.Now < expiration_date)
                                        {
                                            if (Shared.config?.two_step_auth == true)
                                            {

                                                MySqlCommand log_in_session_key_is_validated = connection.CreateCommand();
                                                log_in_session_key_is_validated.CommandText = "SELECT Log_In_Code FROM Log_In_Sessions_Waiting_For_Approval WHERE Log_In_Session_Key = @Log_In_Session_Key";
                                                log_in_session_key_is_validated.Parameters.AddWithValue("Log_In_Session_Key", hashed_key.Item1);

                                                try
                                                {
                                                    DbDataReader log_in_session_key_is_validated_reader = await log_in_session_key_is_validated.ExecuteReaderAsync();
                                                    try
                                                    {
                                                        if (await log_in_session_key_is_validated_reader.ReadAsync() == true)
                                                        {
                                                            serverPayload.response_message = "Log in session key is valid";
                                                        }
                                                        else
                                                        {
                                                            serverPayload.response_message = "Log in session not approved";
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
                                                serverPayload.response_message = "Log in session key is valid";
                                            }
                                        }
                                        else
                                        {
                                            serverPayload.response_message = "Log in session key expired";
                                        }
                                    }
                                    else
                                    {
                                        serverPayload.response_message = "Invalid log in session key";
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
    }
}
