namespace QLVPP.Services.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long? UserId =>
            long.TryParse(_httpContextAccessor.HttpContext?.User.FindFirst("id")?.Value, out var id)
                ? id
                : null;

        public string? UserAccount =>
            _httpContextAccessor.HttpContext?.User.FindFirst("account")?.Value
            ?? _httpContextAccessor.HttpContext?.User.Identity?.Name;

    }
}
