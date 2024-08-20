using MySqlConnector;
using ThetaFTP.Shared.Classes;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Controllers
{
    public class FtpDatabaseController : CRUD_Interface<FtpModel, FtpModel, FtpModel, FtpModel>
    {
        public Task<string?> Delete(FtpModel? value)
        {
            throw new NotImplementedException();
        }

        public Task<string?> Get(FtpModel? value)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> Insert(FtpModel? value)
        {
            string? result = "Internal server error";


            if (value?.fileStream != null)
            {
                if (value?.file_name != null)
                {
                    if (value?.path != null)
                    {
                        MySqlConnection connection = await Shared.mysql.InitiateMySQLConnection();
                        try
                        {
                            //MySqlCommand 
                        }
                        finally
                        {
                            await connection.DisposeAsync();
                        }
                    }
                    else
                    {
                        result = "Invalid path";
                    }
                }
                else
                {
                    result = "Invalid file name";
                }
            }
            else
            {
                result = "Cannot read file content";
            }

            return result;
        }

        public Task<string?> Update(FtpModel? value)
        {
            throw new NotImplementedException();
        }
    }
}
