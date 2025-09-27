using ThetaFTP.Shared.Models;

namespace ThetaFTP.Shared.Classes
{
    public class CRUD_Payload_Strategy<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType> : CRUD_Interface_Payload<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType>
    {
        CRUD_Interface_Payload<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType> strategy;

        public CRUD_Payload_Strategy(CRUD_Interface_Payload<InsertType, GetInfoType, GetType, UpdateType, RenameType, DeleteType> strategy_)
        {
            strategy = strategy_;
        }

        public async Task<PayloadModel?> Delete(DeleteType? value)
        {
            return await strategy.Delete(value);
        }

        public async Task<PayloadModel?> Get(GetType? value)
        {
            return await strategy.Get(value);
        }

        public async Task<PayloadModel?> GetInfo(GetInfoType? value)
        {
            return await strategy.GetInfo(value);
        }

        public async Task<PayloadModel?> Insert(InsertType? value)
        {
            return await strategy.Insert(value);
        }

        public async Task<PayloadModel?> Rename(RenameType? value)
        {
            return await strategy.Rename(value);
        }

        public async Task<PayloadModel?> Update(UpdateType? value)
        {
            return await strategy.Update(value);
        }
    }
}
