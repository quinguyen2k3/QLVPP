using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IRequisitionRepository : IBaseRepository<Requisition>
    {
        Task<List<Requisition>> GetByCreator(string creator);
        Task<List<Requisition>> GetByIds(IEnumerable<long> ids);
    }
}
