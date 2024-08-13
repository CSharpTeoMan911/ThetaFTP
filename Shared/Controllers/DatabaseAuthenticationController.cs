using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;
using System.Data.Common;
using HallRentalSystem.Classes.StructuralAndBehavioralElements.Formaters;
using MySqlConnector;

namespace ThetaFTP.Shared.Controllers
{
    public class DatabaseAuthenticationController : CRUD_Interface<AuthenticationModel, AuthenticationModel, AuthenticationModel, AuthenticationModel>
    {
        public async Task<string?> Delete(AuthenticationModel? value)
        {
            MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

            try
            {

            }
            catch
            {

            }

            throw new NotImplementedException();
        }

        public async Task<string?> Get(AuthenticationModel? value)
        {
            string? response = "Internal server error";

            if (value != null)
            {
                if (value.email != null)
                {
                    if (value.password != null)
                    {

                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

                        MySqlCommand check_if_account_exists_command = connection.CreateCommand();
                        try
                        {
                            check_if_account_exists_command.CommandText = "SELECT Password FROM Credentials WHERE Email = @Email";
                            check_if_account_exists_command.Parameters.AddWithValue("Email", value.email);

                            DbDataReader check_if_account_exists_reader = await check_if_account_exists_command.ExecuteReaderAsync();
                            try
                            {
                                if (await check_if_account_exists_reader.ReadAsync() == true)
                                {
                                    Tuple<string, Type> hashed_password = await Sha512Hasher.Hash(value.password);

                                    if (hashed_password.Item2 != typeof(Exception))
                                    {
                                        if (check_if_account_exists_reader.GetString(0) == hashed_password.Item1)
                                        {
                                            await check_if_account_exists_reader.CloseAsync();

                                            if (Shared.config?.two_step_auth == true)
                                            {
                                                MySqlCommand check_if_account_is_approved_command = connection.CreateCommand();
                                                try
                                                {
                                                    check_if_account_is_approved_command.CommandText = "SELECT Account_Validation_Code FROM Accounts_Waiting_For_Approval WHERE Email = @Email";
                                                    check_if_account_is_approved_command.Parameters.AddWithValue("Email", value.email);
                                                    DbDataReader check_if_account_is_approved_command_reader = await check_if_account_is_approved_command.ExecuteReaderAsync();
                                                    try
                                                    {
                                                        if (await check_if_account_is_approved_command_reader.ReadAsync() == true)
                                                        {
                                                            response = "Account not approved";
                                                            goto End;
                                                        }

                                                        await check_if_account_is_approved_command_reader.CloseAsync();
                                                    }
                                                    finally
                                                    {
                                                        await check_if_account_is_approved_command_reader.DisposeAsync();
                                                    }
                                                }
                                                finally
                                                {
                                                    await check_if_account_is_approved_command.DisposeAsync();
                                                    await connection.DisposeAsync();
                                                }
                                            }


                                            string log_in_session_key = await CodeGenerator.GenerateKey(40);
                                            Tuple<string, Type> hashed_log_in_session_key = await Sha512Hasher.Hash(log_in_session_key);

                                            MySqlCommand insert_log_in_key_command = connection.CreateCommand();
                                            try
                                            {
                                                insert_log_in_key_command.CommandText = "INSERT INTO Log_In_Sessions VALUES(@Log_In_Session_Key, @Email, @Expiration_Date)";
                                                insert_log_in_key_command.Parameters.AddWithValue("Log_In_Session_Key", log_in_session_key);
                                                insert_log_in_key_command.Parameters.AddWithValue("Email", value.email);
                                                insert_log_in_key_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddDays(2));

                                                if (Shared.config?.two_step_auth == true)
                                                {;
                                                    string log_in_code = await CodeGenerator.GenerateKey(10);
                                                    Tuple<string, Type> hashed_log_in_code = await Sha512Hasher.Hash(log_in_code);

                                                    MySqlCommand insert_log_in_code_command = connection.CreateCommand();
                                                    try
                                                    {
                                                        insert_log_in_code_command.CommandText = "INSERT INTO Log_In_Sessions_Waiting_For_Approval VALUES(@Log_In_Code, @Log_In_Session_Key, @Expiration_Date)";
                                                        insert_log_in_code_command.Parameters.AddWithValue("Log_In_Code", log_in_code);
                                                        insert_log_in_code_command.Parameters.AddWithValue("Log_In_Session_Key", log_in_session_key);
                                                        insert_log_in_code_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddDays(2));

                                                        response = "Check the code sent to your email address to approve your log in session";
                                                    }
                                                    finally
                                                    {
                                                        await insert_log_in_code_command.DisposeAsync();
                                                    }
                                                }
                                                else
                                                {
                                                    response = log_in_session_key;
                                                }
                                            }
                                            finally
                                            {
                                                await insert_log_in_key_command.DisposeAsync();
                                            }

                                        End:;
                                        }
                                        else
                                        {
                                            response = "Invalid password";
                                        }
                                    }
                                    else
                                    {
                                        response = "Internal server error";
                                    }
                                }
                                else
                                {
                                    response = "Invalid email";
                                }
                            }
                            finally
                            {
                                await check_if_account_exists_reader.DisposeAsync();
                            }
                        }
                        finally
                        {
                            await check_if_account_exists_command.DisposeAsync();
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
                                            reader_command.CommandText = "SELECT Password FROM Credentials WHERE Email = @Email";
                                            reader_command.Parameters.AddWithValue("Email", value?.email);
                                            DbDataReader reader = await reader_command.ExecuteReaderAsync();

                                            try
                                            {
                                                if (await reader.ReadAsync() == false)
                                                {
                                                    await reader.CloseAsync();
                                                    MySqlCommand credentials_insertion_command = connection.CreateCommand();
                                                    Tuple<string, Type> hash_result = await Sha512Hasher.Hash(value?.password);

                                                    if (hash_result.Item2 != typeof(Exception))
                                                    {
                                                        try
                                                        {
                                                            credentials_insertion_command.CommandText = "INSERT INTO credentials VALUES(@Email, @Password)";
                                                            credentials_insertion_command.Parameters.AddWithValue("Email", value?.email);
                                                            credentials_insertion_command.Parameters.AddWithValue("Password", hash_result.Item1);
                                                            await credentials_insertion_command.ExecuteNonQueryAsync();

                                                            if (Shared.config?.two_step_auth == true)
                                                            {
                                                                MySqlCommand account_validation_code_insertion_command = connection.CreateCommand();

                                                                try
                                                                {
                                                                    string key = await CodeGenerator.GenerateKey(10);
                                                                    Tuple<string, Type> key_hash_result = await Sha512Hasher.Hash(key);

                                                                    if (key_hash_result.Item2 != typeof(Exception))
                                                                    {
                                                                        bool smtps_operation_result = await SMTPS_Service.SendSMTPS(value?.email, "Account approval", $"Account approval key: {key}");
                                                                        account_validation_code_insertion_command.CommandText = "INSERT INTO Accounts_Waiting_For_Approval VALUES(@Account_Validation_Code, @Email, @Expiration_Date)";
                                                                        account_validation_code_insertion_command.Parameters.AddWithValue("Email", value?.email);
                                                                        account_validation_code_insertion_command.Parameters.AddWithValue("Account_Validation_Code", key_hash_result.Item1);
                                                                        account_validation_code_insertion_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddHours(1));
                                                                        await account_validation_code_insertion_command.ExecuteNonQueryAsync();

                                                                        if (smtps_operation_result == true)
                                                                        {
                                                                            response = "Registration successful";
                                                                        }
                                                                        else
                                                                        {
                                                                            response = "Internal server error";
                                                                        }
                                                                    }
                                                                }
                                                                finally
                                                                {
                                                                    await account_validation_code_insertion_command.DisposeAsync();
                                                                }
                                                            }
                                                            else
                                                            {
                                                                response = "Registration Successful";
                                                            }
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
                                            finally
                                            {
                                                await reader.CloseAsync();
                                                await reader.DisposeAsync();
                                            }
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

        public async Task<string?> Update(AuthenticationModel? value)
        {
            MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

            try
            {

            }
            catch
            {

            }

            throw new NotImplementedException();
        }
    }
}
