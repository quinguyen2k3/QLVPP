using QLVPP.DTOs.Projection;
using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<List<Product>> GetAllIsActivated();
        Task<List<Product>> GetByIds(IEnumerable<long> ids);
        Task<List<Product>> GetByWarehouseId(long id);
        Task<List<ProductReportProj>> GetTotalIn(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        );

        Task<List<ProductReportProj>> GetTotalOut(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        );

        Task<List<ProductReportProj>> GetTotalReturnAsync(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        );
    }
}
