namespace QLVPP.DTOs.Response
{
    public class InventorySnapshotRes
    {
        public long Id { get; set; }
        public string WarehouseName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateOnly SnapshotDate { get; set; }
        public List<InventorySnapshotItemRes> Items { get; set; } = new();
    }
}
