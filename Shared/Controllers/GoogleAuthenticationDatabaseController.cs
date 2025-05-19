using ThetaFTP.Shared;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;
using MySql.Data.MySqlClient;

namespace ThetaFTP.Shared.Controllers
{
    public class GoogleAuthenticationDatabaseController : CRUD_Auth_Interface<string, string, string, string, string, string>
    {
        public Task<PayloadModel?> Delete(string? value)
        {
            throw new NotImplementedException();
        }

        public Task<PayloadModel?> Get(string? value)
        {
            throw new NotImplementedException();
        }

        public Task<PayloadModel?> GetInfo(string? value)
        {
            throw new NotImplementedException();
        }

        public async Task<PayloadModel?> Insert(string? value)
        {
            PayloadModel payload = new PayloadModel();

            MySqlConnection client = await Shared.mysql.InitiateMySQLConnection();
            if (client.State == System.Data.ConnectionState.Open)
            {
                try
                {

                }
                catch (Exception e)
                {
                    payload.result = "Internal server error";
                    payload.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                }
                finally
                {
                    client.Dispose();
                }
            }

            throw new NotImplementedException();
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
