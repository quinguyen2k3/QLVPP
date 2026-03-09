using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class PositionReq
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;
        public string? Note { get; set; }
        public bool IsActivated { get; set; }
    }
}
