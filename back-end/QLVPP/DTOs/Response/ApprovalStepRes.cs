namespace QLVPP.DTOs.Request
{
    public class ApprovalStepRes
    {
        public long Id { get; set; }
        public int StepOrder { get; set; }
        public string? StepName { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public string ScopeDisplay { get; set; } = string.Empty;
    }
}
