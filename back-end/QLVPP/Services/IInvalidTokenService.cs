using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IInvalidTokenService
    {
        Task<InvalidTokenRes?> GetById(string id);
    }
}
