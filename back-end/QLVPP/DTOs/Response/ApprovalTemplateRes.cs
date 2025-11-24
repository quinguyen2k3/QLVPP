namespace QLVPP.DTOs.Request
{
    public class ApprovalTemplateRes
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Code { get; set; }
        public string NoteType { get; set; } = string.Empty;
        public string NoteTypeDisplay { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public List<ApprovalStepRes> Steps { get; set; } = new();
    }
}
