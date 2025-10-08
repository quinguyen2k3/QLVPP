using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class RequisitionReq
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }
        [Required(ErrorMessage = "EmployeeId is required")]
        public long EmployeeId { get; set; }
        [Required(ErrorMessage = "IsActivated status is required")]
        public bool IsActivated { get; set; }

        public List<RequisitionItemReq> Items { get; set; } = new();
    }
}
