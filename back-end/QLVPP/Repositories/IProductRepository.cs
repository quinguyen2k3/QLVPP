using QLVPP.DTOs.Result;
using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<List<Product>> GetAllIsActivated();
        Task<List<Product>> GetByIds(IEnumerable<long> ids);
        Task<List<Product>> GetByWarehouseId(long id);
        Task<List<ProductReportResult>> GetTotalIn(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        );

        Task<List<ProductReportResult>> GetTotalOut(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        );
    }
}
