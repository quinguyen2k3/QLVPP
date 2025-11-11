namespace QLVPP.DTOs.Request
{
    public class TransferReq
    {
        public long FromWarehouseId { get; set; }
        public long ToWarehouseId { get; set; }
        public DateOnly TransferredDate { get; set; }
        public string? Note { get; set; }
        public List<TransferReqDetail> Items { get; set; } = new List<TransferReqDetail>();
    }
}
