namespace QLVPP.DTOs.Response
{
    public class AuthRes
    {
        public string AccessToken { get; set; } = string.Empty;
        public bool Authenticated { get; set; }
        public bool? IsActivated { get; set; }
        public long? UserId { get; set; }
    }
}
