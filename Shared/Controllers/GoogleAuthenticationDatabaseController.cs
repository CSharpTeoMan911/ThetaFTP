using Google.Rpc;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Diagnostics;
using ThetaFTP.Shared;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Formatters;
using ThetaFTP.Shared.Models;
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace ThetaFTP.Shared.Controllers
{
    public class GoogleAuthenticationDatabaseController : CRUD_Auth_Interface<GAuthModel, string, string, string, string, string>
    {
        public Task<PayloadModel?> Delete(string? value)
        {
            throw new NotImplementedException();
        }

        public async Task<PayloadModel?> Get(string? value)
        {
            PayloadModel payload = new PayloadModel();



            return payload;
        }

        public Task<PayloadModel?> GetInfo(string? value)
        {
            throw new NotImplementedException();
        }

        public async Task<PayloadModel?> Insert(GAuthModel? value)
        {
            PayloadModel payload = new PayloadModel();
            
            if (value != null)
            {
                if (value.uuid != null)
                {
                    if (value.email != null)
                    {
                        MySqlConnection client = await Shared.mysql.InitiateMySQLConnection();

                        if (client.State == System.Data.ConnectionState.Open)
                        {
                            try
                            {
                                MySqlCommand command = client.CreateCommand();
                                try
                                {
                                    command.CommandText = "SELECT Google_UID FROM google_credentials WHERE Google_UID = @Google_UID";
                                    command.Parameters.AddWithValue("Google_UID", value.uuid);

                                    DbDataReader reader = await command.ExecuteReaderAsync();
                                    try
                                    {
                                        MySqlCommand insert_command = client.CreateCommand();
                                        try
                                        {
                                            if ((await reader.ReadAsync()) == false)
                                            {
                                                await reader.CloseAsync();
                                                insert_command.CommandText = "INSERT INTO google_credentials VALUE(@Google_UID)";
                                                insert_command.Parameters.AddWithValue("Google_UID", value.uuid);
                                                await insert_command.ExecuteNonQueryAsync();

                                                payload.result = "Sign up successful";
                                                payload.StatusCode = System.Net.HttpStatusCode.OK;
                                            }
                                            else
                                            {
                                                await reader.CloseAsync();
                                                payload.result = "Sign in successful";
                                                payload.StatusCode = System.Net.HttpStatusCode.OK;
                                            }

                                            if (Shared.sha512 != null)
                                            {
                                                string? log_in_session_key = await CodeGenerator.GenerateKey(40);
                                                string log_in_session_key_hash_result = await Shared.sha512.Hash(log_in_session_key);

                                                MySqlCommand session_insertion_command = client.CreateCommand();

                                                try
                                                {
                                                    session_insertion_command.CommandText = "INSERT INTO google_log_in_sessions VALUE(@Log_In_Session_Key, @Google_UID, @Expiration_Date)";
                                                    session_insertion_command.Parameters.AddWithValue("Log_In_Session_Key", log_in_session_key_hash_result);
                                                    session_insertion_command.Parameters.AddWithValue("Google_UID", value?.uuid);
                                                    session_insertion_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddDays(2));

                                                    if (Shared.configurations?.twoStepAuth == true)
                                                    {
                                                        string? code = await CodeGenerator.GenerateKey(10);
                                                        string code_hash_result = await Shared.sha512.Hash(code);

                                                        bool smtps_operation_result = SMTPS_Service.SendSMTPS(value?.email, "Log in authorisation", $"Login code: {code}");

                                                        if (smtps_operation_result == true)
                                                        {
                                                            await session_insertion_command.ExecuteNonQueryAsync();

                                                            MySqlCommand code_insertion_command = client.CreateCommand();

                                                            try
                                                            {
                                                                code_insertion_command.CommandText = "INSERT INTO google_log_in_session_waiting_for_approval VALUES(@Log_In_Code, @Log_In_Session_Key, @Expiration_Date)";
                                                                code_insertion_command.Parameters.AddWithValue("Log_In_Code", code_hash_result);
                                                                code_insertion_command.Parameters.AddWithValue("Log_In_Session_Key", log_in_session_key_hash_result);
                                                                code_insertion_command.Parameters.AddWithValue("Expiration_Date", DateTime.Now.AddMinutes(2));
                                                                await code_insertion_command.ExecuteNonQueryAsync();
                                                               

                                                                payload.payload = log_in_session_key;
                                                                payload.StatusCode = System.Net.HttpStatusCode.OK;
                                                            }
                                                            catch(Exception e)
                                                            {
                                                                Logging.Message(e, "Error inserting session into database", "Check if the database is running and the connection is valid", "GoogleAuthenticationDatabaseController", "Insert", Logging.LogType.Error);
                                                                payload.result = "Internal server error";
                                                                payload.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                                                            }
                                                            finally
                                                            {
                                                                await code_insertion_command.DisposeAsync();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            payload.result = "Internal server error";
                                                            payload.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        payload.payload = log_in_session_key;
                                                        await session_insertion_command.ExecuteNonQueryAsync();
                                                        payload.StatusCode = System.Net.HttpStatusCode.OK;
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Logging.Message(e, "Error inserting session into database", "Check if the database is running and the connection is valid", "GoogleAuthenticationDatabaseController", "Insert", Logging.LogType.Error);
                                                    payload.result = "Internal server error";
                                                    payload.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                                                }
                                                finally
                                                {
                                                    await session_insertion_command.DisposeAsync();
                                                }
                                            }
                                            else
                                            {
                                                payload.result = "Internal server error";
                                                payload.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Logging.Message(e, "Error inserting Google credentials into database", "Check if the database is running and the connection is valid", "GoogleAuthenticationDatabaseController", "Insert", Logging.LogType.Error);
                                            payload.result = "Internal server error";
                                            payload.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                                        }
                                        finally
                                        {
                                            await insert_command.DisposeAsync();
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Logging.Message(e, "Error reading from Google credentials database", "Check if the database is running and the connection is valid", "GoogleAuthenticationDatabaseController", "Insert", Logging.LogType.Error);
                                        payload.result = "Internal server error";
                                        payload.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                                    }
                                    finally
                                    {
                                        await reader.DisposeAsync();
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logging.Message(e, "Error executing command on Google credentials database", "Check if the database is running and the connection is valid", "GoogleAuthenticationDatabaseController", "Insert", Logging.LogType.Error);
                                    payload.result = "Internal server error";
                                    payload.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                                }
                                finally
                                {
                                    await command.DisposeAsync();
                                }
                            }
                            catch (Exception e)
                            {
                                Logging.Message(e, "Error creating command for Google credentials database", "Check if the database is running and the connection is valid", "GoogleAuthenticationDatabaseController", "Insert", Logging.LogType.Error);
                                payload.result = "Internal server error";
                                payload.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                            }
                            finally
                            {
                                await client.DisposeAsync();
                            }
                        }
                    }
                }
            }

            return payload;
        }

        public Task<PayloadModel?> Rename(string? value)
        {
            throw new NotImplementedException();
        }

        public Task<PayloadModel?> Update(string? value)
        {
            throw new NotImplementedException();
        }
    }
}
