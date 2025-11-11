using QLVPP.DTOs.Projection;
using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IStockOutRepository : IBaseRepository<StockOut>
    {
        Task<List<StockOut>> GetByWarehouseId(long id);
        Task<List<StockOut>> GetPendingByWarehouseId(long id);
        Task<List<StockOut>> GetApprovedByDepartmentId(long id);
        Task<List<StockOut>> GetByCreator(string creator);
    }
}
