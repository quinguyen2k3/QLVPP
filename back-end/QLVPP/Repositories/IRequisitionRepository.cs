using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IRequisitionRepository : IBaseRepository<Requisition>
    {
        Task<List<Requisition>> GetByCurrentApproverId(long id);
        Task<List<Requisition>> GetByCreator(string creator);
    }
}
