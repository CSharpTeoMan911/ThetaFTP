using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Data;

namespace ThetaFTP.Shared.Controllers
{
    public class DatabaseServerFunctionsController
    {
        public async Task DeleteDatabaseCache()
        {
            List<string> expired_accounts = new List<string>();

            MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();

            if (connection.State == ConnectionState.Open)
            {
                try
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
                                expired_accounts.Add(email);
                            }

                            await expired_accounts_reader.CloseAsync();
                        }
                        finally
                        {
                            await expired_accounts_reader.DisposeAsync();
                        }
                    }
                    finally
                    {
                        await select_expired_accounts_command.DisposeAsync();
                    }


                    for (int i = 0; i < expired_accounts.Count; i++)
                    {
                        MySqlCommand delete_expired_account_command = connection.CreateCommand();
                        try
                        {
                            delete_expired_account_command.CommandText = "DELETE FROM credentials WHERE Email = @Email";
                            delete_expired_account_command.Parameters.AddWithValue("Email", expired_accounts.ElementAt(i));
                            await delete_expired_account_command.ExecuteNonQueryAsync();
                        }
                        finally
                        {
                            await delete_expired_account_command.DisposeAsync();
                        }
                    }
                }
                finally
                {
                    await connection.DisposeAsync();
                }
            }
        }
    }
}
