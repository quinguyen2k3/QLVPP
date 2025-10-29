using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class SupplierReq
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }

        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Phone number format is invalid")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "IsActivated status is required")]
        public bool IsActivated { get; set; }
    }
}
