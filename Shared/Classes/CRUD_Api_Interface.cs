using Microsoft.AspNetCore.Mvc;

namespace ThetaFTP.Shared.Classes
{
    public interface CRUD_Api_Interface<InsertTypeQuery, InsertTypeBody, GetTypeQuery, GetTypeBody, UpdateTypeQuery, UpdateTypeBody, DeleteTypeQuery, DeleteTypeBody>
    { 
        public Task<ActionResult?> Get(GetTypeQuery? query, GetTypeBody? body);
        public Task<ActionResult?> Insert(InsertTypeQuery? query, InsertTypeBody? body);
        public Task<ActionResult?> Update(UpdateTypeQuery? query, UpdateTypeBody? body);
        public Task<ActionResult?> Delete(DeleteTypeQuery? query, DeleteTypeBody? body);
    }
}
