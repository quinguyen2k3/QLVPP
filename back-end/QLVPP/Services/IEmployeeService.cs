using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeRes>> GetAll();
        Task<List<EmployeeRes>> GetAllActived();
        Task<EmployeeRes?> GetById(long id);
        Task<EmployeeRes> Create(EmployeeReq request);
        Task<EmployeeRes?> Update(long id, EmployeeReq request);
    }
}
