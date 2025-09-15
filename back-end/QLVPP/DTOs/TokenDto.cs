using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs
{
    public class TokenDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;
        [Required]
        public string RefreshToken {  get; set; } = string.Empty;
    }
}
