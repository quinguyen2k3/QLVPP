using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IRoleService
    {
        Task<List<RoleRes>> GetAll();
    }
}
