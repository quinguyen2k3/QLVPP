namespace QLVPP.Services.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long GetUserId()
        {
            var idClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;
            if (long.TryParse(idClaim, out var id))
                return id;

            throw new InvalidOperationException("User ID not found in the current context.");
        }

        public string GetUserAccount()
        {
            var accountClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("account")?.Value;
            if (!string.IsNullOrEmpty(accountClaim))
                return accountClaim;

            var name = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(name))
                return name;

            return "SYSTEM";
        }

        public bool IsUserAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public long GetWarehouseId()
        {
            var warehouseClaim = _httpContextAccessor
                .HttpContext?.User?.FindFirst("warehouseId")
                ?.Value;
            if (long.TryParse(warehouseClaim, out var id))
                return id;

            throw new InvalidOperationException("Warehouse ID not found for the current user.");
        }
    }
}
