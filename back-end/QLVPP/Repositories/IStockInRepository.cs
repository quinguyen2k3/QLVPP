using QLVPP.DTOs.Request;
using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IStockInRepository : IBaseRepository<StockIn>
    {
        Task<List<StockIn>> GetByConditions(StockInFilterReq filter);
        Task<bool> HasUnapprovedDocs(long warehouseId, DateOnly toDate);
        Task<StockIn?> GetByCode(string code);
    }
}
