using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Security
{
    public interface IReturnRepository : IBaseRepository<Return>
    {
        public Task<List<Return>> GetAllIsActivated();
    }
}