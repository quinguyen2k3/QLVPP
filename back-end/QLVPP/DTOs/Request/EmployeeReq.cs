using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using QLVPP.DTOs.Response;

namespace QLVPP.DTOs.Request
{
    public class EmployeeReq
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email format is invalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(
            @"^(?:\+84|0)[1-9][0-9]{8}$",
            ErrorMessage = "Phone number must be 10 digits and start with 0 or +84"
        )]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account is required")]
        [StringLength(50, ErrorMessage = "Account cannot exceed 50 characters")]
        [RegularExpression(
            "^[a-zA-Z0-9_.]+$",
            ErrorMessage = "Account must not contain special characters or diacritics"
        )]
        public string Account { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Password field is required.")]
        [RegularExpression(
            "^.{8,}$",
            ErrorMessage = "Password must be at least 8 characters long."
        )]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public long DepartmentId { get; set; }

        public long? WarehouseId { get; set; }

        [Required(ErrorMessage = "IsActivated status is required")]
        public bool IsActivated { get; set; } = true;
    }
}
