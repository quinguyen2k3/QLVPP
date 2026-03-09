using QLVPP.Constants.Types;

namespace QLVPP.DTOs.Request
{
    public class StockOutFilterReq
    {
        public StockOutType? Type { get; set; }
        public long? FromWarehouseId { get; set; }
        public long? ToWarehouseId { get; set; }
        public long? DepartmentId { get; set; }
        public List<string>? Statuses { get; set; }
        public string? CreatedBy { get; set; }
        public bool? IsActivated { get; set; }
        public bool OrderByDesc { get; set; } = true;
    }
}
