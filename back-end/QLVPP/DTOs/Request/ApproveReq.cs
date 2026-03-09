namespace QLVPP.DTOs.Request
{
    public class ApproveReq
    {
        public long RequestId { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
