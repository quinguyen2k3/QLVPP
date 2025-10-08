namespace QLVPP.Services
{
    public interface ICurrentUserService
    {
        long? UserId { get; }
        long? WarehouseId { get; }
        string? UserAccount { get; }
        bool IsAuthenticated { get; }
    }
}
