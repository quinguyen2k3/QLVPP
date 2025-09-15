

using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class EmployeeReq
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Account { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public long DepartmentId { get; set; }
        [Required]
        public bool IsActived { get; set; } = true;
    }
}
