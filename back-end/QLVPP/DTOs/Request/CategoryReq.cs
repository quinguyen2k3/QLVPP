using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class CategoryReq
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }

        [Required(ErrorMessage = "IsActivated status is required")]
        public bool IsActivated { get; set; }
    }
}
