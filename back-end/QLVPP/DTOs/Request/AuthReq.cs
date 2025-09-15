using System.ComponentModel.DataAnnotations;

namespace QLVPP.DTOs.Request
{
    public class AuthReq
    {
        [Required]
        public string Account {  get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
