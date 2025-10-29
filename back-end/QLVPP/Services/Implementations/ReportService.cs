using QLVPP.DTOs.Response;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ReportService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<List<DeptUsageRes>> GetReportUsage(DateOnly startDate, DateOnly endDate)
        {
            var userWarehouseId = _currentUserService.GetWarehouseId();
            var data = await _unitOfWork.Report.GetUsageReport(userWarehouseId, startDate, endDate);

            var report = data.GroupBy(r => new { r.DepartmentId, r.DepartmentName })
                .Select(g => new DeptUsageRes
                {
                    DepartmentId = g.Key.DepartmentId,
                    DepartmentName = g.Key.DepartmentName,
                    Items = g.Select(item => new UsageItemRes
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            Quantity = item.Quantity,
                        })
                        .ToList(),
                })
                .ToList();
            return report;
        }
    }
}
