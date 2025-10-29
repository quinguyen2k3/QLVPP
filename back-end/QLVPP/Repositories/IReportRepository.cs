using QLVPP.DTOs.Projection;

namespace QLVPP.Repositories
{
    public interface IReportRepository
    {
        Task<List<UsageSummaryProj>> GetUsageReport(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        );
    }
}
