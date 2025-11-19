namespace QLVPP.DTOs.Request
{
    public class ApprovalStepReq
    {
        public long Id { get; set; }
        public int StepOrder { get; set; }
        public long AssignedToId { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
