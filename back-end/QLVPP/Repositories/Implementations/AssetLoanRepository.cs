using Microsoft.EntityFrameworkCore;
using QLVPP.Data;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class AssetLoanRepository(AppDbContext context) : BaseRepository<AssetLoan>(context), IAssetLoanRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<List<AssetLoan>> GetByDepartmentIdAndWarehouseId(long departmentId, long warehouseId)
        {
            return await _context.AssetLoans
                .Where(a => a.DeliveryDetail.Delivery.DepartmentId == departmentId)
                .Where(a => a.DeliveryDetail.Delivery.WarehouseId== warehouseId) 
                .Include(a => a.DeliveryDetail.Delivery)
                .Include(a => a.DeliveryDetail.Product) 
                .ToListAsync();
        }

        public override async Task<AssetLoan?> GetById(params object[] id)
        {
            var deliveryDetailId = (long)id[0];

            return await _context.AssetLoans
                .Include(a => a.DeliveryDetail.Delivery)
                .Include(a => a.DeliveryDetail.Product)
                .FirstOrDefaultAsync(a => a.DeliveryDetailId == deliveryDetailId);
        }
    }
}