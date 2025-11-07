using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IDeliveryService
    {
        Task<List<DeliveryRes>> GetByWarehouse();
        Task<List<DeliveryRes>> GetAllByMyself();
        Task<DeliveryRes?> GetById(long id);
        Task<DeliveryRes> Create(DeliveryReq request);
        Task<DeliveryRes?> Update(long id, DeliveryReq request);
        Task<DeliveryRes?> Dispatch(long id, DeliveryReq request);
        Task<bool> Delete(long id);
    }
}
