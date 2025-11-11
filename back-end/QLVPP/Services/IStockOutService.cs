using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IStockOutService
    {
        Task<List<StockOutRes>> GetByWarehouse();
        Task<List<StockOutRes>> GetApprovedForDepartment();
        Task<List<StockOutRes>> GetPendingByWarehouse();
        Task<List<StockOutRes>> GetAllByMyself();
        Task<StockOutRes?> GetById(long id);
        Task<StockOutRes> Create(StockOutReq request);
        Task<StockOutRes?> Update(long id, StockOutReq request);
        Task<StockOutRes?> Approve(long id, StockOutReq request);
        Task<StockOutRes?> Receive(long id);
        Task<bool> Delete(long id);
    }
}
