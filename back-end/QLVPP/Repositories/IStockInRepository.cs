using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IStockInRepository : IBaseRepository<StockIn>
    {
        Task<List<StockIn>> GetByCreator(string creator);
        Task<List<StockIn>> GetByWarehouseId(long id);
        Task<List<StockIn>> GetPendingByWarehouseId(long id);
    }
}
