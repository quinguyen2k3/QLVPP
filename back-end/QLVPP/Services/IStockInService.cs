using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IStockInService
    {
        Task<List<StockInRes>> GetByWarehouse();
        Task<List<StockInRes>> GetPendingByWarehouse();
        Task<List<StockInRes>> GetAllByMyself();
        Task<StockInRes?> GetById(long id);
        Task<StockInRes> Create(StockInReq request);
        Task<StockInRes?> Update(long id, StockInReq request);
        Task<StockInRes?> Approve(long id, StockInReq request);
        Task<bool> Delete(long id);
    }
}
