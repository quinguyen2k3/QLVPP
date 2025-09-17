using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface ISupplierService
    {
        Task<List<SupplierRes>> GetAll();
        Task<List<SupplierRes>> GetAllActived();
        Task<SupplierRes?> GetById(long id);
        Task<SupplierRes> Create(SupplierReq request);
        Task<SupplierRes?> Update(long id, SupplierReq request);
    }
}
