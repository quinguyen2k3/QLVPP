namespace QLVPP.DTOs.Response
{
    public class ApprovalStepRes
    {
        public long Id { get; set; }
        public int StepOrder { get; set; }
        public long AssignedToId { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime? ApprovedAt { get; set; }
    }
}
