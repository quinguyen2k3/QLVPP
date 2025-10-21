namespace QLVPP.DTOs.Response
{
    public class EmployeeRes
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Account { get; set; }
        public long? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public long? WarehouseId { get; set; }
        public bool? IsActivated { get; set; }
    }
}
