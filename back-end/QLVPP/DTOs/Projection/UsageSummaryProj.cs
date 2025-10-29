namespace QLVPP.DTOs.Projection
{
    public class UsageSummaryProj
    {
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public long ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
