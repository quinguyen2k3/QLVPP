namespace QLVPP.DTOs.Response
{
    public class InventorySnapshotItemRes
    {
        public string ProductName { get; set; } = null!;
        public int OpeningQty { get; set; }
        public int TotalIn { get; set; }
        public int TotalOut { get; set; }
        public int ClosingQty { get; set; }
    }
}
