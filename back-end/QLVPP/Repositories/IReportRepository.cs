using QLVPP.DTOs.Result;

namespace QLVPP.Repositories
{
    public interface IReportRepository
    {
        Task<List<UsageSummaryResult>> GetUsageReport(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        );
    }
}
