namespace QLVPP.DTOs.Response
{
    public class ApprovalConfigRes
    {
        public long Id { get; set; }
        public string ApprovalType { get; set; } = string.Empty;
        public string ApprovalTypeDisplay { get; set; } = string.Empty;
        public int? RequiredApprovals { get; set; }
        public List<ApproverRes> Approvers { get; set; } = new();
    }
}
