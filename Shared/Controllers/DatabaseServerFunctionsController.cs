using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Data;
using Serilog;

namespace ThetaFTP.Shared.Controllers
{
    public class DatabaseServerFunctionsController
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
            MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    MySqlConnection command_execution_connection = await Shared.mysql.InitiateMySQLConnection();

                    try
                    {
                        if (command_execution_connection.State == ConnectionState.Open)
                        {
                            MySqlCommand select_expired_accounts_command = connection.CreateCommand();

                            try
                            {
                                select_expired_accounts_command.CommandText = "SELECT Email FROM accounts_waiting_for_approval WHERE Expiration_Date < NOW();";
                                DbDataReader expired_accounts_reader = await select_expired_accounts_command.ExecuteReaderAsync();
                                try
                                {
                                    while (await expired_accounts_reader.ReadAsync() == true)
                                    {
                                        string email = expired_accounts_reader.GetString(0);

                                        MySqlCommand delete_expired_account_command = command_execution_connection.CreateCommand();
                                        try
                                        {
                                            delete_expired_account_command.CommandText = "DELETE FROM credentials WHERE Email = @Email";
                                            delete_expired_account_command.Parameters.AddWithValue("Email", email);
                                            await delete_expired_account_command.ExecuteNonQueryAsync();
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error(e, "Error deleting expired accounts");
                                        }
                                        finally
                                        {
                                            await delete_expired_account_command.DisposeAsync();
                                        }
                                    }

                                    await expired_accounts_reader.CloseAsync();
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e, "Error deleting expired accounts");
                                }
                                finally
                                {
                                    await expired_accounts_reader.DisposeAsync();
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Error deleting expired accounts");
                            }
                            finally
                            {
                                await select_expired_accounts_command.DisposeAsync();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error deleting expired accounts");
                    }
                    finally
                    {
                        await command_execution_connection.DisposeAsync();
                    }

                }
                catch(Exception e)
                {
                    Log.Error(e, "Error deleting expired accounts");
                }
                finally
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private async Task DeleteAccountsWaitingForDeletion()
        {
            MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    MySqlConnection command_execution_connection = await Shared.mysql.InitiateMySQLConnection();

                    try
                    {
                        if (command_execution_connection.State == ConnectionState.Open)
                        {
                            MySqlCommand select_expired_deletion_command = connection.CreateCommand();

                            try
                            {
                                select_expired_deletion_command.CommandText = "SELECT Account_Deletion_Code FROM accounts_waiting_for_deletion WHERE Expiration_Date < NOW();";
                                DbDataReader expired_deletion_reader = await select_expired_deletion_command.ExecuteReaderAsync();
                                try
                                {
                                    while (await expired_deletion_reader.ReadAsync() == true)
                                    {
                                        string code = expired_deletion_reader.GetString(0);

                                        MySqlCommand delete_expired_deletion_command = command_execution_connection.CreateCommand();
                                        try
                                        {
                                            delete_expired_deletion_command.CommandText = "DELETE FROM accounts_waiting_for_deletion WHERE Account_Deletion_Code = @Account_Deletion_Code";
                                            delete_expired_deletion_command.Parameters.AddWithValue("Account_Deletion_Code", code);
                                            await delete_expired_deletion_command.ExecuteNonQueryAsync();
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error(e, "Error deleting accounts deletion requests");
                                        }
                                        finally
                                        {
                                            await delete_expired_deletion_command.DisposeAsync();
                                        }
                                    }

                                    await expired_deletion_reader.CloseAsync();
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e, "Error deleting accounts deletion requests");
                                }
                                finally
                                {
                                    await expired_deletion_reader.DisposeAsync();
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Error deleting accounts deletion requests");
                            }
                            finally
                            {
                                await select_expired_deletion_command.DisposeAsync();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error deleting accounts deletion requests");
                    }
                    finally
                    {
                        await command_execution_connection.DisposeAsync();
                    }

                }
                catch (Exception e)
                {
                    Log.Error(e, "Error deleting accounts deletion requests");
                }
                finally
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private async Task DeleteAccountsWaitingForPasswordChange()
        {
            MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    MySqlConnection command_execution_connection = await Shared.mysql.InitiateMySQLConnection();

                    try
                    {
                        if (command_execution_connection.State == ConnectionState.Open)
                        {
                            MySqlCommand select_expired_password_update_command = connection.CreateCommand();

                            try
                            {
                                select_expired_password_update_command.CommandText = "SELECT Account_Password_Change_Code FROM accounts_waiting_for_password_change WHERE Expiration_Date < NOW();";
                                DbDataReader expired_password_update_reader = await select_expired_password_update_command.ExecuteReaderAsync();
                                try
                                {
                                    while (await expired_password_update_reader.ReadAsync() == true)
                                    {
                                        string code = expired_password_update_reader.GetString(0);

                                        MySqlCommand delete_expired_password_update_command = command_execution_connection.CreateCommand();
                                        try
                                        {
                                            delete_expired_password_update_command.CommandText = "DELETE FROM accounts_waiting_for_password_change WHERE Account_Password_Change_Code = @Account_Password_Change_Code";
                                            delete_expired_password_update_command.Parameters.AddWithValue("Account_Password_Change_Code", code);
                                            await delete_expired_password_update_command.ExecuteNonQueryAsync();
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error(e, "Error deleting password update requests");
                                        }
                                        finally
                                        {
                                            await delete_expired_password_update_command.DisposeAsync();
                                        }
                                    }

                                    await expired_password_update_reader.CloseAsync();
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e, "Error deleting password update requests");
                                }
                                finally
                                {
                                    await expired_password_update_reader.DisposeAsync();
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Error deleting password update requests");
                            }
                            finally
                            {
                                await select_expired_password_update_command.DisposeAsync();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error deleting password update requests");
                    }
                    finally
                    {
                        await command_execution_connection.DisposeAsync();
                    }

                }
                catch (Exception e)
                {
                    Log.Error(e, "Error deleting password update requests");
                }
                finally
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private async Task DeleteLogInSessionWaitingForApproval()
        {
            MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    MySqlConnection command_execution_connection = await Shared.mysql.InitiateMySQLConnection();

                    try
                    {
                        if (command_execution_connection.State == ConnectionState.Open)
                        {
                            MySqlCommand select_expired_log_in_sesssions_waiting_for_approval_command = connection.CreateCommand();

                            try
                            {
                                select_expired_log_in_sesssions_waiting_for_approval_command.CommandText = "SELECT Log_In_Code, Log_In_Session_Key FROM log_in_sessions_waiting_for_approval WHERE Expiration_Date < NOW();";
                                DbDataReader expired_log_in_sesssions_waiting_for_approval_reader = await select_expired_log_in_sesssions_waiting_for_approval_command.ExecuteReaderAsync();
                                try
                                {
                                    while (await expired_log_in_sesssions_waiting_for_approval_reader.ReadAsync() == true)
                                    {
                                        string code = expired_log_in_sesssions_waiting_for_approval_reader.GetString(0);
                                        string key = expired_log_in_sesssions_waiting_for_approval_reader.GetString(1);

                                        MySqlCommand delete_expired_log_in_sesssions_waiting_for_approval_command = command_execution_connection.CreateCommand();
                                        try
                                        {
                                            delete_expired_log_in_sesssions_waiting_for_approval_command.CommandText = "DELETE FROM log_in_sessions_waiting_for_approval WHERE Log_In_Code = @Log_In_Code";
                                            delete_expired_log_in_sesssions_waiting_for_approval_command.Parameters.AddWithValue("Log_In_Code", code);
                                            await delete_expired_log_in_sesssions_waiting_for_approval_command.ExecuteNonQueryAsync();
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error(e, "Error deleting log in requests");
                                        }
                                        finally
                                        {
                                            await delete_expired_log_in_sesssions_waiting_for_approval_command.DisposeAsync();
                                        }

                                        MySqlCommand delete_expired_log_in_sesssions_command = command_execution_connection.CreateCommand();
                                        try
                                        {
                                            delete_expired_log_in_sesssions_command.CommandText = "DELETE FROM log_in_sessions WHERE Log_In_Session_Key = @Log_In_Session_Key";
                                            delete_expired_log_in_sesssions_command.Parameters.AddWithValue("Log_In_Session_Key", key);
                                            await delete_expired_log_in_sesssions_command.ExecuteNonQueryAsync();
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error(e, "Error deleting log in requests");
                                        }
                                        finally
                                        {
                                            await delete_expired_log_in_sesssions_command.DisposeAsync();
                                        }
                                    }

                                    await expired_log_in_sesssions_waiting_for_approval_reader.CloseAsync();
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e, "Error deleting log in requests");
                                }
                                finally
                                {
                                    await expired_log_in_sesssions_waiting_for_approval_reader.DisposeAsync();
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Error deleting log in requests");
                            }
                            finally
                            {
                                await select_expired_log_in_sesssions_waiting_for_approval_command.DisposeAsync();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error deleting log in requests");
                    }
                    finally
                    {
                        await command_execution_connection.DisposeAsync();
                    }

                }
                catch (Exception e)
                {
                    Log.Error(e, "Error deleting log in requests");
                }
                finally
                {
                    await connection.DisposeAsync();
                }
            }
        }

        private async Task DeleteLogInSession()
        {
            MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

            if (connection.State == ConnectionState.Open)
            {
                try
                {
                    MySqlConnection command_execution_connection = await Shared.mysql.InitiateMySQLConnection();

                    try
                    {
                        if (command_execution_connection.State == ConnectionState.Open)
                        {
                            MySqlCommand select_expired_log_in_sessions_command = connection.CreateCommand();

                            try
                            {
                                select_expired_log_in_sessions_command.CommandText = "SELECT Log_In_Session_Key FROM log_in_sessions WHERE Expiration_Date < NOW();";
                                DbDataReader expired_log_in_sessions_reader = await select_expired_log_in_sessions_command.ExecuteReaderAsync();
                                try
                                {
                                    while (await expired_log_in_sessions_reader.ReadAsync() == true)
                                    {
                                        string key = expired_log_in_sessions_reader.GetString(0);

                                        MySqlCommand delete_expired_log_in_sessions_command = command_execution_connection.CreateCommand();
                                        try
                                        {
                                            delete_expired_log_in_sessions_command.CommandText = "DELETE FROM log_in_sessions WHERE Log_In_Session_Key = @Log_In_Session_Key";
                                            delete_expired_log_in_sessions_command.Parameters.AddWithValue("Log_In_Session_Key", key);
                                            await delete_expired_log_in_sessions_command.ExecuteNonQueryAsync();
                                        }
                                        catch (Exception e)
                                        {
                                            Log.Error(e, "Error deleting log in sessions");
                                        }
                                        finally
                                        {
                                            await delete_expired_log_in_sessions_command.DisposeAsync();
                                        }
                                    }

                                    await expired_log_in_sessions_reader.CloseAsync();
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e, "Error deleting log in sessions");
                                }
                                finally
                                {
                                    await expired_log_in_sessions_reader.DisposeAsync();
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error(e, "Error deleting log in sessions");
                            }
                            finally
                            {
                                await select_expired_log_in_sessions_command.DisposeAsync();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error deleting log in sessions");
                    }
                    finally
                    {
                        await command_execution_connection.DisposeAsync();
                    }

                }
                catch (Exception e)
                {
                    Log.Error(e, "Error deleting log in sessions");
                }
                finally
                {
                    await connection.DisposeAsync();
                }
            }
        }
    }
}
