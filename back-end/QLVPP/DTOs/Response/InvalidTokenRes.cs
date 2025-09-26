namespace QLVPP.DTOs.Response
{
    public class InvalidTokenRes
    {
        public long Id { get; set; }
        public string Jti { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime RevokedAt { get; set; }
        public string RevokedBy { get; set; } = string.Empty;
    }
}
