using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class ChangePassReq
    {
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Confirm password does not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
