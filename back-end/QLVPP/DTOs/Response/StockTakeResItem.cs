namespace QLVPP.DTOs.Response
{
    public class StockTakeResItem
    {
        public long ProductId { get; set; }
        public int SysQty { get; set; }
        public int ActualQty { get; set; }
    }
}
