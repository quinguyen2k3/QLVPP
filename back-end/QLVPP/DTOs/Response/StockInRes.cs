using QLVPP.Constants.Types;

namespace QLVPP.DTOs.Response
{
    public class StockInRes
    {
        public long Id { get; set; }
        public DateOnly StockInDate { get; set; }
        public StockInType Type { get; set; }
        public long RequesterId { get; set; }
        public long WarehouseId { get; set; }
        public long? FromWarehouseId { get; set; }
        public long? FromDepartmentId { get; set; }
        public string? ReferenceId { get; set; }
        public long? SupplierId { get; set; }
        public bool IsActivated { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public List<StockInItemRes> Items { get; set; } = new();

        public string Code
        {
            get { return $"PNK-{StockInDate:yy}-{Id.ToString("D6")}"; }
        }
    }
}
