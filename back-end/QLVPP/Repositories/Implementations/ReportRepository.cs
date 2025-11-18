using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.DTOs.Projection;

namespace QLVPP.Repositories.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UsageSummaryProj>> GetUsageReport(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        )
        {
            var deliveredItems =
                from detail in _context.StockOutDetails
                join delivery in _context.StockOuts on detail.DeliveryId equals delivery.Id
                where
                    delivery.WarehouseId == warehouseId
                    && delivery.Status == StockOutStatus.Approved
                    && delivery.DeliveryDate >= startDate
                    && delivery.DeliveryDate <= endDate
                select new TransactionItemProj
                {
                    DepartmentId = delivery.DepartmentId,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                };

            var returnedItems =
                from detail in _context.ReturnDetails
                join ret in _context.Returns on detail.ReturnId equals ret.Id
                where
                    ret.WarehouseId == warehouseId
                    && ret.Status == ReturnStatus.Returned
                    && ret.ReturnDate >= startDate
                    && ret.ReturnDate <= endDate
                select new TransactionItemProj
                {
                    DepartmentId = ret.DepartmentId,
                    ProductId = detail.ProductId,
                    Quantity = -detail.ReturnedQuantity,
                };
            var allTransactions = deliveredItems.Concat(returnedItems);

            var query =
                from trans in allTransactions
                join department in _context.Departments on trans.DepartmentId equals department.Id
                join product in _context.Products on trans.ProductId equals product.Id
                group trans by new
                {
                    DepartmentId = department.Id,
                    DepartmentName = department.Name,
                    ProductId = product.Id,
                    ProductName = product.Name,
                } into g
                select new UsageSummaryProj
                {
                    DepartmentId = g.Key.DepartmentId,
                    DepartmentName = g.Key.DepartmentName,
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Quantity = g.Sum(t => t.Quantity),
                };

            return await query
                .OrderBy(r => r.DepartmentName)
                .ThenBy(r => r.ProductName)
                .ToListAsync();
        }
    }
}
