using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IProductService
    {
        Task<List<ProductRes>> GetAll();
        Task<List<ProductRes>> GetAllActived();
        Task<ProductRes?> GetById(long id);
        Task<ProductRes> Create(ProductReq request);
        Task<ProductRes?> Update(long id, ProductReq request);
    }
}
