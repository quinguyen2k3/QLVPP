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
    }
}
