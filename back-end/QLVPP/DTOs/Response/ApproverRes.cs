namespace QLVPP.DTOs.Response
{
    public class ApproverRes
    {
        public long Id { get; set; }
        public long EmployeeId { get; set; }
        public int Priority { get; set; }
        public bool IsMandatory { get; set; }
    }
}
