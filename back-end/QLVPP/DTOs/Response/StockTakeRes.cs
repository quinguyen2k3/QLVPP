using QLVPP.DTOs.Request;

namespace QLVPP.DTOs.Response
{
    public class StockTakeRes
    {
        public long Id;
        public string Purpose = null!;
        public DateTime CreatedDate;
        public long PerformanceId { get; set; }
        public required List<StockTakeResItem> Items { get; set; }
    }
}
