namespace QLVPP.Services
{
    public interface ICurrentUserService
    {
        long? UserId { get; }
        string? UserAccount { get; }
        bool IsAuthenticated { get; }
    }
}
