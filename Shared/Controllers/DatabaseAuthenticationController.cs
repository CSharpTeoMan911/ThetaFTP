using System.Data.Entity;
using System.Data.SQLite;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;
using ThetaFTP.Shared;
using System.Data.Common;
using HallRentalSystem.Classes.StructuralAndBehavioralElements.Formaters;
using System.CodeDom;

namespace ThetaFTP.Shared.Controllers
{
    public class DatabaseAuthenticationController : CRUD_Interface<AuthenticationModel, AuthenticationModel, AuthenticationModel, AuthenticationModel>
    {
        public async Task<string?> Delete(AuthenticationModel? value)
        {
            SQLiteConnection connection = await Shared.sql_lite.InitiateSQLiteConnection();

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
            string? response = "Unsuccessful";

            if (value != null)
            {
                if (value.email != null)
                {
                    if (value.password != null)
                    {
                        SQLiteConnection connection = await Shared.sql_lite.InitiateSQLiteConnection();

                        try
                        {
                            Tuple<string, Type> hash_result = await Sha512Hasher.Hash(value?.password);

                            SQLiteCommand credentials_validation_command = connection.CreateCommand();
                            credentials_validation_command.CommandText = "SELECT Password FROM Credentials WHERE Email = @Email";
                            credentials_validation_command.Parameters.AddWithValue("Email", value?.email);

                            try
                            {
                                if (hash_result.Item2 != typeof(Exception))
                                {
                                    DbDataReader reader = await credentials_validation_command.ExecuteReaderAsync();

                                    try
                                    {
                                        if (await reader.ReadAsync() == true)
                                        {
                                            if (reader.GetString(0) == hash_result.Item1)
                                            {

                                                int recursions = 10;

                                            Key_Generation:
                                                string key = await LogInSessionKeyGenerator.GenerateKey();
                                                Tuple<string, Type> key_hash_result = await Sha512Hasher.Hash(key);

                                                if (key_hash_result.Item2 != typeof(Exception))
                                                {
                                                    SQLiteCommand log_in_session_key_validation_command = connection.CreateCommand();

                                                    try
                                                    {
                                                        log_in_session_key_validation_command.CommandText = "SELECT Email FROM Log_In_Sessions WHERE Log_In_Session_Key = @Log_In_Session_Key";
                                                        log_in_session_key_validation_command.Parameters.AddWithValue("Log_In_Session_Key", key_hash_result);

                                                        DbDataReader key_reader = await log_in_session_key_validation_command.ExecuteReaderAsync();

                                                        try
                                                        {
                                                            if (await key_reader.ReadAsync() == true)
                                                            {
                                                                recursions--;
                                                                if (recursions >= 0)
                                                                    goto Key_Generation;
                                                            }
                                                            else
                                                            {
                                                                SQLiteCommand log_in_session_key_insertion_command = connection.CreateCommand();

                                                                try
                                                                {
                                                                    log_in_session_key_insertion_command.CommandText = "INSERT INTO Log_In_Sessions VALUES(@Log_In_Session_Key, @Email, @Expiration_Date)";
                                                                    log_in_session_key_insertion_command.Parameters.AddWithValue("Log_In_Session_Key", key_hash_result.Item1);
                                                                    log_in_session_key_insertion_command.Parameters.AddWithValue("Email", value?.email);
                                                                    log_in_session_key_insertion_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddDays(2));

                                                                    await log_in_session_key_insertion_command.ExecuteNonQueryAsync();


                                                                    if (Shared.config?.two_step_auth == true)
                                                                    {
                                                                        // ADD 2 STEP AUTH PROCEDURE
                                                                    }

                                                                    response = key;
                                                                }
                                                                finally
                                                                {
                                                                    await log_in_session_key_insertion_command.DisposeAsync();
                                                                }
                                                            }
                                                        }
                                                        finally
                                                        {
                                                            await key_reader.DisposeAsync();
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        await log_in_session_key_validation_command.DisposeAsync();
                                                    }
                                                }
                                                else
                                                {
                                                    response = key_hash_result.Item1;
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
                                    finally
                                    {
                                        await reader.DisposeAsync();
                                    }
                                }
                                else
                                {
                                    response = hash_result.Item1;
                                }
                            }
                            finally
                            {
                                await credentials_validation_command.DisposeAsync();
                            }
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
            string? response = "Unsuccessful";

            if (value != null)
            {
                if (value?.email != null)
                {
                    if (value?.email.Length <= 100)
                    {
                        if (value?.password != null)
                        {
                            if (value?.password.Length >= 10)
                            {
                                if (value?.password.Length <= 100)
                                {
                                    SQLiteConnection connection = await Shared.sql_lite.InitiateSQLiteConnection();
                                    SQLiteCommand reader_command = connection.CreateCommand();

                                    try
                                    {
                                        reader_command.CommandText = "SELECT Password FROM Credentials WHERE Email = @Email";
                                        reader_command.Parameters.AddWithValue("Email", value?.email);
                                        DbDataReader reader = await reader_command.ExecuteReaderAsync();

                                        try
                                        {
                                            if (await reader.ReadAsync() == false)
                                            {
                                                SQLiteCommand credentials_insertion_command = connection.CreateCommand();
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
                                                            // ADD 2 STEP AUTH PROCEDURE
                                                        }
                                                        else
                                                        {
                                                            response = "Successful";
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        await credentials_insertion_command.DisposeAsync();
                                                    }
                                                }
                                                else
                                                {
                                                    response = hash_result.Item1;
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
                                Console.WriteLine($"Length: {value?.password.Length}");
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
                response = "Invalid credentials";
            }

            return response;
        }

        public async Task<string?> Update(AuthenticationModel? value)
        {
            SQLiteConnection connection = await Shared.sql_lite.InitiateSQLiteConnection();

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
