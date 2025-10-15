using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class LogoutReq
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;
    }
}
