using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IUnitService
    {
        Task<List<UnitRes>> GetAll();
        Task<List<UnitRes>> GetAllActivated();
        Task<UnitRes?> GetById(long id);
        Task<UnitRes> Create(UnitReq request);
        Task<UnitRes?> Update(long id, UnitReq request);
    }
}
