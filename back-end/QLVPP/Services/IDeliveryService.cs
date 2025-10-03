using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IDeliveryService
    {
        Task<List<DeliveryRes>> GetAll();
        Task<List<DeliveryRes>> GetAllActived();
        Task<DeliveryRes?> GetById(long id);
        Task<DeliveryRes> Create(DeliveryReq request);
        Task<DeliveryRes?> Update(long id, DeliveryReq request);
        Task<DeliveryRes?> Dispatch(long id, DeliveryReq request);
    }
}
