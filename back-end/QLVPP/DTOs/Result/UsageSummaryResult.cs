namespace QLVPP.DTOs.Result
{
    public class UsageSummaryResult
    {
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
