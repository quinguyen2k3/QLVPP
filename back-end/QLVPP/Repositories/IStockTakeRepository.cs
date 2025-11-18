using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IStockTakeRepository : IBaseRepository<StockTake>
    {
        Task<List<StockTake>> GetByWarehouseId(long id);
    }
}
