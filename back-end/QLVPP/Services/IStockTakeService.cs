using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Services
{
    public interface IStockTakeService
    {
        public Task<StockTakeRes> Create(StockTakeReq request);
        public Task<StockTakeRes?> Update(long id, StockTakeReq request);
        public Task<List<StockTakeRes>> GetByWarehouse();
        public Task<StockTakeRes?> GetById(long id);
        public Task<bool> Cancel(long id);
        public Task<bool> Approve(long id);
        public Task<bool> Delete(long id);
    }
}
