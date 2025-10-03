using AutoMapper;
using Azure.Core;
using QLVPP.Constants;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeliveryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DeliveryRes> Create(DeliveryReq request)
        {
            var delivery = _mapper.Map<Delivery>(request);
            delivery.Status = DeliveryStatus.Pending;

            await _unitOfWork.Delivery.Add(delivery);
            await _unitOfWork.SaveChanges();

            var response = _mapper.Map<DeliveryRes>(delivery);
            return response;
        }

        public async Task<List<DeliveryRes>> GetAll()
        {
            var deliveries = await _unitOfWork.Delivery.GetAll();
            return _mapper.Map<List<DeliveryRes>>(deliveries);
        }

        public async Task<List<DeliveryRes>> GetAllActived()
        {
            var deliveries = await _unitOfWork.Delivery.GetAllIsActived();
            return _mapper.Map<List<DeliveryRes>>(deliveries);
        }

        public async Task<DeliveryRes?> GetById(long id)
        {
            var delivery = await _unitOfWork.Delivery.GetById(id);
            return delivery == null ? null : _mapper.Map<DeliveryRes>(delivery);
        }

        public async Task<DeliveryRes?> Update(long id, DeliveryReq request)
        {
            var delivery = await _unitOfWork.Delivery.GetById(id);
            if (delivery == null) return null;

            if (delivery.Status != DeliveryStatus.Pending)
            {
                throw new InvalidOperationException("Delivery is not in Pending status and cannot be updated.");
            }

            _mapper.Map(request, delivery);

            await _unitOfWork.Delivery.Update(delivery);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<DeliveryRes>(delivery);
        }

        public async Task<DeliveryRes?> Dispatch(long id, DeliveryReq request)
        {

            var delivery = await _unitOfWork.Delivery.GetById(id);
            if (delivery == null)
            {
                return null;
            }

            if (delivery.Status != DeliveryStatus.Pending)
            {
                throw new InvalidOperationException("Delivery can only be dispatched if the status is Pending.");
            }

            foreach (var item in delivery.DeliveryDetails)
            {
                var inventory = await _unitOfWork.Inventory.GetByKey(request.WarehouseId, item.ProductId);
                if (inventory == null || inventory.Quantity < item.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product '{item.Product?.Name ?? "unknown"}'.");
                }

                inventory.Quantity -= item.Quantity;
                await _unitOfWork.Inventory.Update(inventory);
            }

            delivery.Status = DeliveryStatus.Complete;

            await _unitOfWork.Delivery.Update(delivery);

            await _unitOfWork.SaveChanges();

            return _mapper.Map<DeliveryRes>(delivery);
        }
    }
}
