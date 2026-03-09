namespace QLVPP.DTOs.Request
{
    public class InventorySnapshotFilterReq
    {
        public long? WarehouseId { get; set; }

        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public bool? IsActivated { get; set; }
        public bool OrderByDesc { get; set; } = true;
    }
}
