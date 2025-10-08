namespace QLVPP.DTOs.Response
{
    public class WarehouseRes
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }
        public bool IsActivated { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
