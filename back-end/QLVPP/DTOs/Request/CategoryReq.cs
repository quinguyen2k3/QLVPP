using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class CategoryReq
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Note cannot exceed 200 characters")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "IsActived status is required")]
        public bool IsActived { get; set; }
    }
}
