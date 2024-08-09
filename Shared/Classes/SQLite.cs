
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;

namespace ThetaFTP.Shared.Classes
{
    public class SQLite
    {
        private static SQLiteConnection? connection;

        public void InitiateSQLiteDatabase()
        {
            try
            {
                if (File.Exists("Thetha_FTP.db") == false)
                {
                    File.Copy("Thetha_FTP (inactive).db", "Thetha_FTP.db", true);
                }
            }
            catch { }
        }

        public async Task<SQLiteConnection> InitiateSQLiteConnection()
        {
            connection = new SQLiteConnection("Data Source= Thetha_FTP.db; Version = 3; New = True; Compress = True;");

            try
            {
                await connection.OpenAsync();
            }
            catch { }
            return connection;
        }
    }
}
