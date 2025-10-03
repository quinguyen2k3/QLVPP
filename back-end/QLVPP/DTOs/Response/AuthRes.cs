namespace QLVPP.DTOs.Response
{
    public class AuthRes
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool Authenticated { get; set; }
        public bool? IsActived { get; set; }
    }
}
