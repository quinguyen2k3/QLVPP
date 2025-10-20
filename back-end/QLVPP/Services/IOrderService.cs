using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface IOrderService
    {
        Task<List<OrderRes>> GetAll();
        Task<List<OrderRes>> GetAllActivated();
        Task<OrderRes?> GetById(long id);
        Task<OrderRes> Create(OrderReq request);
        Task<OrderRes?> Update(long id, OrderReq request);
        Task<OrderRes?> Received(long id, OrderReq request);
        Task<bool> Delete(long id);
    }
}
