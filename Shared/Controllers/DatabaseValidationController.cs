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
                                switch (value?.validationType)
                                {
                                    case Shared.ValidationType.AccountAuthorisation:
                                        validation_command.CommandText = "SELECT Email FROM Accounts_Waiting_For_Approval WHERE Account_Validation_Code = @Account_Validation_Code";
                                        validation_command.Parameters.AddWithValue("Account_Validation_Code", hashed_key.Item1);
                                        break;
                                    case Shared.ValidationType.LogInSessionAuthorisation:
                                        validation_command.CommandText = "SELECT Log_In_Session_Key FROM Log_In_Sessions_Waiting_For_Approval WHERE Log_In_Code = @Log_In_Code";
                                        validation_command.Parameters.AddWithValue("Log_In_Session_Key", hashed_key.Item1);
                                        break;
                                }

                                DbDataReader reader = await validation_command.ExecuteReaderAsync();
                                try
                                {
                                    if (await reader.ReadAsync() == true)
                                    {
                                        string? extracted_value = reader.GetString(0);
                                        await reader.CloseAsync();

                                        switch (value?.validationType)
                                        {
                                            case Shared.ValidationType.AccountAuthorisation:
                                                if (extracted_value != value?.email)
                                                {
                                                    serverPayload.response_message = "Invalid code";
                                                    goto End;
                                                }
                                                break;
                                            case Shared.ValidationType.LogInSessionAuthorisation:
                                                if (extracted_value != value?.code)
                                                {
                                                    serverPayload.response_message = "Invalid code";
                                                    goto End;
                                                }
                                                break;
                                        }


                                        MySqlCommand approval_command = connection.CreateCommand();
                                        try
                                        {
                                            switch (value?.validationType)
                                            {
                                                case Shared.ValidationType.AccountAuthorisation:
                                                    approval_command.CommandText = "DELETE FROM Accounts_Waiting_For_Approval WHERE Account_Validation_Code = @Account_Validation_Code";
                                                    approval_command.Parameters.AddWithValue("Account_Validation_Code", hashed_key.Item1);
                                                    break;
                                                case Shared.ValidationType.LogInSessionAuthorisation:
                                                    approval_command.CommandText = "DELETE FROM Log_In_Sessions_Waiting_For_Approval WHERE Log_In_Code = @Log_In_Code";
                                                    approval_command.Parameters.AddWithValue("Log_In_Session_Key", hashed_key.Item1);
                                                    break;
                                            }

                                            await approval_command.ExecuteNonQueryAsync();

                                            switch (value?.validationType)
                                            {
                                                case Shared.ValidationType.AccountAuthorisation:

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

                                                    break;
                                                case Shared.ValidationType.LogInSessionAuthorisation:
                                                    serverPayload.response_message = extracted_value;
                                                    break;
                                            }
                                        }
                                        finally
                                        {
                                            await approval_command.DisposeAsync();
                                        }

                                    End:;
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
    }
}
