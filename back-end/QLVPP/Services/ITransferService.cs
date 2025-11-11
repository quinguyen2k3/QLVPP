using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;

namespace QLVPP.Services
{
    public interface ITransferService
    {
        Task<List<TransferRes>> GetAllByMyself();
        Task<List<TransferRes>> GetPendingFromWarehouse();
        Task<List<TransferRes>> GetApprovedForWarehouse();
        Task<TransferRes?> GetById(long id);
        Task<TransferRes> Create(TransferReq request);
        Task<TransferRes> Approve(long id, TransferReq request);
        Task<TransferRes> Receive(long id, TransferReq request);
    }
}
