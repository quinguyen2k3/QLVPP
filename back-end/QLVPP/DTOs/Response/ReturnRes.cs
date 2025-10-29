namespace QLVPP.DTOs.Response
{
    public class ReturnRes
    {
        public long Id { get; set; }

        public long WarehouseId { get; set; }

        public long DepartmentId { get; set; }
        public string? Note { get; set; }
        public List<ReturnItemRes> Items { get; set; } = new List<ReturnItemRes>();
    }
}
