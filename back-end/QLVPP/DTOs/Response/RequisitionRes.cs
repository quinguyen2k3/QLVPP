using QLVPP.DTOs.Request;

namespace QLVPP.DTOs.Response
{
    public class RequisitionRes
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty; 
        public string? Note { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedBy { get; set; }

        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public List<RequisitionItemRes> Items { get; set; } = new();

    }
}
