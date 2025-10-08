using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IAssetLoanRepository : IBaseRepository<AssetLoan>
    {
        Task<List<AssetLoan>> GetByDepartmentIdAndWarehouseId(long departmentId, long warehouseId);
    }
}