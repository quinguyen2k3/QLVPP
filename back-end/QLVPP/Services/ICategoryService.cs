using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryRes>> GetAll();
        Task<List<CategoryRes>> GetAllActived();
        Task<CategoryRes?> GetById(long id);
        Task<CategoryRes> Create(CategoryReq request);
        Task<CategoryRes?> Update(long id, CategoryReq request);
    }
}
