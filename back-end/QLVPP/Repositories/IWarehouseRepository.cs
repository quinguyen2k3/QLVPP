using QLVPP.Models;

namespace QLVPP.Repositories
{
    public interface IWarehouseRepository : IBaseRepository<Warehouse>
    {
        Task<List<Warehouse>> GetAllIsActivated();
    }
}
