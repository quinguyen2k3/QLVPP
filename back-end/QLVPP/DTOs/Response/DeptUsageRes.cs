namespace QLVPP.DTOs.Response
{
    public class DeptUsageRes
    {
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public List<UsageItemRes> Items { get; set; } = new List<UsageItemRes>();
    }
}
