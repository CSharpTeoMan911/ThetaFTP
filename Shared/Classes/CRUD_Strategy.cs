
namespace ThetaFTP.Shared.Classes
{
    public class CRUD_Strategy<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType> : CRUD_Interface<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType>
    {
        CRUD_Interface<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType> strategy;

        public CRUD_Strategy(CRUD_Interface<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType> strategy_)
        {
            strategy = strategy_;
        }

        public async Task<string?> Delete(DeleteType? value)
        {
            return await strategy.Delete(value);
        }

        public async Task<string?> Get(GetType? value)
        {
            return await strategy.Get(value);
        }

        public async Task<string?> GetInfo(GetInfoType? value)
        {
            return await strategy.GetInfo(value);
        }

        public async Task<string?> Insert(InsertType? value)
        {
            return await strategy.Insert(value);
        }

        public async Task<string?> Rename(RenameType? value)
        {
            return await strategy.Rename(value);
        }

        public async Task<string?> Update(UpdateType? value)
        {
            return await strategy.Update(value);
        }
    }
}
