using QLVPP.Constants.Types;

namespace QLVPP.DTOs.Request
{
    public class MaterialRequestFilterReq
    {
        public List<string>? Statuses { get; set; }
        public List<RequestType>? RequestTypes { get; set; }
        public long? WarehouseId { get; set; }
        public long? RequesterId { get; set; }
        public long? ApproverId { get; set; }
        public bool IsActivated { get; set; } = true;
        public string? CreatedBy { get; set; }
        public bool OrderByDesc { get; set; } = false;
    }
}
