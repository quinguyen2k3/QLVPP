using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface ISupplierRepository : IBaseRepository<Supplier>
    {
        Task<List<Supplier>> GetAllIsActived();
    }
}
