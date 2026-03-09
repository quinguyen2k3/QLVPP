namespace QLVPP.DTOs.Request
{
    public class StockInFilterReq
    {
        public string? CreatedBy { get; set; }
        public long? WarehouseId { get; set; }
        public List<string>? Statuses { get; set; }
        public bool? IsActivated { get; set; } = true;
        public bool OrderByDesc { get; set; } = true;
    }
}
