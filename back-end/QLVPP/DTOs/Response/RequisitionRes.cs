namespace QLVPP.DTOs.Response
{
    public class RequisitionRes
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string? Note { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedBy { get; set; }
        public long RequesterId { get; set; }
        public long DepartmentId { get; set; }
        public bool IsActivated { get; set; }
        public List<RequisitionItemRes> Items { get; set; } = new();
        public ApprovalConfigRes Config { get; set; } = new();
    }
}
