using Org.BouncyCastle.Tls;
using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public interface CRUD_Interface_Payload<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType>
    {
        public Task<ServerPayloadModel?> GetInfo(GetInfoType? value);
        public Task<ServerPayloadModel?> Get(GetType? value);
        public Task<ServerPayloadModel?> Insert(InsertType? value);
        public Task<ServerPayloadModel?> Update(UpdateType? value);
        public Task<ServerPayloadModel?> Rename(RenameType? value);
        public Task<ServerPayloadModel?> Delete(DeleteType? value);
    }
}
