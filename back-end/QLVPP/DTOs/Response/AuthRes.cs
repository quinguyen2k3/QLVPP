namespace QLVPP.DTOs.Response
{
    public class AuthRes
    {
        public string AccessToken { get; set; } = string.Empty;
        public bool Authenticated { get; set; }
        public bool? IsActivated { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
