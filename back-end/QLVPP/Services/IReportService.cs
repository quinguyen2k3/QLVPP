using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IReportService
    {
        Task<List<DeptUsageRes>> GetReportUsage(DateOnly startDate, DateOnly endDate);
    }
}
