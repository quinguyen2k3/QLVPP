namespace QLVPP.DTOs.Request
{
    public class StockTakeResItem
    {
        public long ProductId { get; set; }
        public int SysQty { get; set; }
        public int ActualQty { get; set; }
        public int Difference { get; set; }
    }
}
