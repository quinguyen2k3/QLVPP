using QLVPP.Constants.Types;

namespace QLVPP.DTOs.Response
{
    public class MaterialRequestRes
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long RequesterId { get; set; }
        public long WarehouseId { get; set; }
        public long DepartmentId { get; set; }
        public long ApproverId { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ReferenceId { get; set; } = string.Empty;
        public RequestType Type { get; set; }
        public DateOnly ExpectedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<MaterialRequestItemRes> Items { get; set; } = new();
    }
}
