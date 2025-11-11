namespace QLVPP.DTOs.Response
{
    public class TransferRes
    {
        public long Id { get; set; }
        public long FromWarehouseId { get; set; }
        public long ToWarehouseId { get; set; }
        public DateOnly TransferredDate { get; set; }
        public string? Note { get; set; }

        public List<TransferResDetail> Items { get; set; } = new List<TransferResDetail>();
    }
}
