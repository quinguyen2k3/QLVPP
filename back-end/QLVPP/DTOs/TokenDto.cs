using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs
{
    public class TokenDto
    {
        [Required(ErrorMessage = "Access Token is required")]
        public string AccessToken { get; set; } = string.Empty;
        [Required(ErrorMessage = "Refresh Token is required")]
        public string RefreshToken {  get; set; } = string.Empty;
    }
}
