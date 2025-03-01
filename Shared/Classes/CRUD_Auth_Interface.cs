using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public interface CRUD_Auth_Interface<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType>
    {
        public Task<PayloadModel?> GetInfo(GetInfoType? value);
        public Task<PayloadModel?> Get(GetType? value);
        public Task<PayloadModel?> Insert(InsertType? value);
        public Task<PayloadModel?> Update(UpdateType? value);
        public Task<PayloadModel?> Rename(RenameType? value);
        public Task<PayloadModel?> Delete(DeleteType? value);
    }
}
