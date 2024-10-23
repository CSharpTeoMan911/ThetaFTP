namespace ThetaFTP.Shared.Classes
{
    public interface CRUD_Interface<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType>
    {
        public Task<string?> GetInfo(GetInfoType? value);
        public Task<string?> Get(GetType? value);
        public Task<string?> Insert(InsertType? value);
        public Task<string?> Update(UpdateType? value);
        public Task<string?> Rename(RenameType? value);
        public Task<string?> Delete(DeleteType? value);
    }
}
