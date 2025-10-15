using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<List<Product>> GetAllIsActivated();
        Task<List<Product>> GetByIds(IEnumerable<long> ids);
        Task<List<Product>> GetByWarehouseId(long id);
        Task<List<ProductReportRes>> GetTotalIn(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        );

        Task<List<ProductReportRes>> GetTotalOut(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        );

        Task<List<ProductReportRes>> GetTotalReturnAsync(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        );
    }
}
