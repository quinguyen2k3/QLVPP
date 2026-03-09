using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.DTOs.Result;

namespace QLVPP.Repositories.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UsageSummaryResult>> GetUsageReport(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        )
        {
            var deliveredItems =
                from detail in _context.StockOutDetails
                join delivery in _context.StockOuts on detail.StockOutId equals delivery.Id
                where
                    delivery.WarehouseId == warehouseId
                    && delivery.Status == StockOutStatus.Approved
                    && delivery.StockOutDate >= startDate
                    && delivery.StockOutDate <= endDate
                select new TransactionItemResult
                {
                    DepartmentId = delivery.DepartmentId ?? 0,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                };

            var query =
                from trans in deliveredItems
                join department in _context.Departments on trans.DepartmentId equals department.Id
                join product in _context.Products on trans.ProductId equals product.Id
                group trans by new
                {
                    DepartmentId = department.Id,
                    DepartmentName = department.Name,
                    ProductId = product.Id,
                    ProductName = product.Name,
                } into g
                select new UsageSummaryResult
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
