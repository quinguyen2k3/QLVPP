namespace QLVPP.DTOs.Request
{
    public class InventorySnapshotReq
    {
        public long WarehouseId { get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
    }
}
