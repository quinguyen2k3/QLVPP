using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IDepartmentService
    {
        Task<List<DepartmentRes>> GetAll();
        Task<List<DepartmentRes>> GetAllActived();
        Task<DepartmentRes?> GetById(long id);
        Task<DepartmentRes> Create(DepartmentReq request);
        Task<DepartmentRes?> Update(long id, DepartmentReq request);
    }
}
