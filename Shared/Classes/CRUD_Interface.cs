namespace ThetaFTP.Shared.Classes
{
    public interface CRUD_Interface<InsertType, GetType, UpdateType, DeleteType>
    {
        public Task<string?> Get(GetType? value);
        public Task<string?> Insert(InsertType? value);
        public Task<string?> Update(UpdateType? value);
        public Task<string?> Delete(DeleteType? value);
    }
}
