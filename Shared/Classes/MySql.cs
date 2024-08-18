using MySqlConnector;

namespace ThetaFTP.Shared.Classes
{
    public class MySql
    {
        public async Task<MySqlConnection> InitiateMySQLConnection()
        {
            MySqlConnection connection = new MySqlConnection($"Server={Shared.config?.mysql_server_address};User ID={Shared.config?.mysql_user_id};Password={Shared.config?.mysql_user_password};Database={Shared.config?.mysql_database};");

            try
            {
                await connection.OpenAsync();
            }
            catch
            {
            }
            return connection;
        }
    }
}
