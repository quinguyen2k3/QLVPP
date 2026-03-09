using QLVPP.Constants.Types;
using QLVPP.Models;

namespace QLVPP.DTOs.Response
{
    public class StockOutRes
    {
        public long Id { get; set; }
        public string? Note { get; set; }
        public long DepartmentId { get; set; }
        public long ToWarehouseId { get; set; }
        public long WarehouseId { get; set; }
        public long RequesterId { get; set; }
        public string? ReferenceId { get; set; }
        public DateOnly StockOutDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public StockOutType Type { get; set; }
        public DateTime ReceivedDate { get; set; }
        public List<StockOutResItem> Items { get; set; } = new();

        public string Code
        {
            get { return $"PXK-{StockOutDate:yy}-{Id.ToString("D6")}"; }
        }
    }
}
