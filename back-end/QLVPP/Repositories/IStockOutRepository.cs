using QLVPP.Constants.Status;
using QLVPP.Constants.Types;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Result;
using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IStockOutRepository : IBaseRepository<StockOut>
    {
        Task<List<StockOut>> GetByConditions(StockOutFilterReq filter);
        Task<bool> HasUnfinishedDocs(long warehouseId, DateOnly toDate);
        Task<StockOut?> GetByCode(string code);
    }
}
