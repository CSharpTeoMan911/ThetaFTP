using Microsoft.AspNetCore.Mvc;

namespace ThetaFTP.Shared.Classes
{
    public interface CRUD_Auth_Api_Interface<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType>
    {
        public Task<ActionResult?> GetInfo(GetInfoType? value);
        public Task<ActionResult?> Get(GetType? value);
        public Task<ActionResult?> Insert(InsertType? value);
        public Task<ActionResult?> Update(UpdateType? value);
        public Task<ActionResult?> Rename(RenameType? value);
        public Task<ActionResult?> Delete(DeleteType? value);
    }
}
