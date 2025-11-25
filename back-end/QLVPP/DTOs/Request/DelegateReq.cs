using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class DelegateReq
    {
        [Required(ErrorMessage = "RequisitionId is required")]
        public long RequisitionId { get; set; }

        [Required(ErrorMessage = "DelegateToEmployeeId is required")]
        public long DelegateToEmployeeId { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }
    }
}
