using MySql.Data.MySqlClient;

namespace ThetaFTP.Shared.Classes
{
    public class MySql:Shared
    {
        public async Task<MySqlConnection> InitiateMySQLConnection()
        {
            int connection_timeout = 600;
            if(configurations != null)
                connection_timeout = configurations.ConnectionTimeoutSeconds;

            MySqlConnection connection = new MySqlConnection($"Server={configurations?.mysql_server_address};Port={configurations?.mysql_server_port};User ID={configurations?.mysql_user_id};Password={configurations?.mysql_user_password};Database={configurations?.mysql_database};Connection timeout={connection_timeout}");
            try
            {
                await connection.OpenAsync();
            }
            catch(Exception e)
            {
                Logging.Message(e, "MySql connection error", "Check if the credentials, IP address, port, and username are valid", "JsonFormatter", "JsonSerialiser", Logging.LogType.Error);
            }
            return connection;
        }
    }
}
