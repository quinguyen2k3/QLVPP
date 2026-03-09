using QLVPP.Constants.Types;

namespace QLVPP.DTOs.Request
{
    public class MaterialRequestReq
    {
        public DateOnly ExpectedDate { get; set; }
        public long RequesterId { get; set; }
        public long ApproverId { get; set; }
        public long WarehouseId { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string? ReferenceId { get; set; }
        public RequestType Type { get; set; }
        public List<MaterialRequestReqItem> Items { get; set; } =
            new List<MaterialRequestReqItem>();
    }
}
