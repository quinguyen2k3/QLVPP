namespace QLVPP.DTOs.Result
{
    public class DepartmentInventoryResult
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string UnitId { get; set; } = string.Empty;

        public int TotalReceived { get; set; }
        public int TotalReturned { get; set; }

        public int CurrentQuantity { get; set; }
    }
}
