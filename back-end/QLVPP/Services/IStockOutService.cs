using QLVPP.Constants.Types;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IStockOutService
    {
        Task<List<StockOutRes>> GetByConditions(StockOutFilterReq filter);
        Task<StockOutRes?> GetById(long id);
        Task<StockOutRes> Create(StockOutReq request);
        Task<StockOutRes?> Update(long id, StockOutReq request);
        Task<bool> Approve(long id);
        Task<bool> Receive(long id);
        Task<bool> Cancel(long id);
        Task<bool> Delete(long id);
    }
}
