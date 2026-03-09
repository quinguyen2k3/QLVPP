using QLVPP.DTOs.Request;
using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IMaterialRequestRepository : IBaseRepository<MaterialRequest>
    {
        public Task<List<MaterialRequest>> GetByConditions(MaterialRequestFilterReq filter);
    }
}
