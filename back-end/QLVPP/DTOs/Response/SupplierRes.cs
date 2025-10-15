namespace QLVPP.DTOs.Response
{
    public class SupplierRes
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Note { get; set; }
        public bool IsActivated { get; set; }
    }
}
