using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IPositionService
    {
        Task<List<PositionRes>> GetAll();
        Task<PositionRes> GetById(long id);
        Task<PositionRes> Create(PositionReq request);
        Task<PositionRes> Update(long id, PositionReq request);
    }
}
