using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ApproverReq
    {
        [Required(ErrorMessage = "EmployeeId is required")]
        public long EmployeeId { get; set; }

        [Range(1, 100, ErrorMessage = "Priority must be between 1 and 100")]
        public int Priority { get; set; } = 1;

        public bool IsMandatory { get; set; } = true;
    }
}
