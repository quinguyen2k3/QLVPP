using AutoMapper;
using QLVPP.Constants;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class ReturnService : IReturnService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public ReturnService(IUnitOfWork uniOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _unitOfWork = uniOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<List<ReturnRes>> GetAll()
        {

            var returns = await _unitOfWork.Return.GetAll();
            return _mapper.Map<List<ReturnRes>>(returns);
        }

        public async Task<List<ReturnRes>> GetAllActived()
        {
            var returns = await _unitOfWork.Return.GetAllIsActived();
            return _mapper.Map<List<ReturnRes>>(returns);
        }

        public async Task<ReturnRes?> GetById(long id)
        {
            var assetLoan = await _unitOfWork.AssetLoan.GetById(id);
            return assetLoan == null ? null : _mapper.Map<ReturnRes>(assetLoan);
        }

        public async Task<ReturnRes?> Update(long id, ReturnReq request)
        {
            var returnNote = await _unitOfWork.AssetLoan.GetById(id);
            if (returnNote == null)
                return null;

            _mapper.Map(request, returnNote);

            await _unitOfWork.AssetLoan.Update(returnNote);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<ReturnRes>(returnNote);
        }

        public async Task<ReturnRes?> Returned(long id, ReturnReq request)
        {
            var returnNote = await _unitOfWork.Return.GetById(id);
            if (returnNote == null)
                return null;

            if (returnNote.Status != ReturnStatus.Pending)
            {
                throw new InvalidOperationException("This return note is note pending");
            }

            foreach (var detail in returnNote.ReturnDetails)
            {
                var assetLoan = detail.AssetLoan;
                if (assetLoan == null)
                {
                    throw new InvalidOperationException($"Associated AssetLoan not found for ReturnDetail Id {detail.Id}.");
                }

                int remainingQuantity = assetLoan.IssuedQuantity - assetLoan.ReturnedQuantity - assetLoan.DamagedQuantity;
                if (detail.ReturnedQuantity + detail.DamagedQuantity > remainingQuantity)
                {
                    throw new InvalidOperationException($"Return quantity for asset '{assetLoan.DeliveryDetail.ProductId}' exceeds the outstanding quantity.");
                }

                if (detail.ReturnedQuantity > 0)
                {
                    var inventory = await _unitOfWork.Inventory.GetByKey(request.WarehouseId, assetLoan.DeliveryDetail.ProductId);
                    if (inventory == null)
                        throw new InvalidOperationException("Inventory not found.");

                    inventory.Quantity += detail.ReturnedQuantity;
                    await _unitOfWork.Inventory.Update(inventory);
                }

                assetLoan.ReturnedQuantity += detail.ReturnedQuantity;
                assetLoan.DamagedQuantity += detail.DamagedQuantity;

                await _unitOfWork.AssetLoan.Update(assetLoan);
            }

            returnNote.Status = ReturnStatus.Complete;
            
            await _unitOfWork.Return.Update(returnNote);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<ReturnRes>(returnNote);
        }
    }
}