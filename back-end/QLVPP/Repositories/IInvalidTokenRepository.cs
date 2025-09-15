using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IInvalidTokenRepository : IBaseRepository<InvalidToken>
    {
        Task<bool> Exists(string jti);
    }
}
