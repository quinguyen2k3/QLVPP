namespace QLVPP.DTOs.Request
{
    public class StockTakeReqItem
    {
        public long ProductId { get; set; }
        public int SysQty { get; set; }
        public int ActualQty { get; set; }
    }
}
