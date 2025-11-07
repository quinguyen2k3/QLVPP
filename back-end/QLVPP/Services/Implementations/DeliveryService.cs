using AutoMapper;
using QLVPP.Constants.Status;
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
        private readonly ICurrentUserService _currentUserService;

        public DeliveryService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<DeliveryRes> Create(DeliveryReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (latestSnapshotDate != null && request.DeliveryDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create delivery note on date ({request.DeliveryDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }
            var delivery = _mapper.Map<Delivery>(request);
            delivery.Status = DeliveryStatus.Pending;

            await _unitOfWork.Delivery.Add(delivery);
            await _unitOfWork.SaveChanges();

            var response = _mapper.Map<DeliveryRes>(delivery);
            return response;
        }

        public async Task<List<DeliveryRes>> GetByWarehouse()
        {
            var warehouseId = _currentUserService.GetWarehouseId();
            var deliveries = await _unitOfWork.Delivery.GetByWarehouseId(warehouseId);
            return _mapper.Map<List<DeliveryRes>>(deliveries);
        }

        public async Task<DeliveryRes?> GetById(long id)
        {
            var delivery = await _unitOfWork.Delivery.GetById(id);
            return delivery == null ? null : _mapper.Map<DeliveryRes>(delivery);
        }

        public async Task<DeliveryRes?> Update(long id, DeliveryReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (latestSnapshotDate != null && request.DeliveryDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create delivery note on date ({request.DeliveryDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }

            var delivery = await _unitOfWork.Delivery.GetById(id);
            if (delivery == null)
                return null;

            if (delivery.Status != DeliveryStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Delivery is not in Pending status and cannot be updated."
                );
            }

            _mapper.Map(request, delivery);

            await _unitOfWork.Delivery.Update(delivery);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<DeliveryRes>(delivery);
        }

        public async Task<DeliveryRes?> Dispatch(long id, DeliveryReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();
            if (latestSnapshotDate != null && request.DeliveryDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create delivery note on date ({request.DeliveryDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }

            var delivery = await _unitOfWork.Delivery.GetById(id);
            if (delivery == null)
            {
                return null;
            }

            if (delivery.Status != DeliveryStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Delivery can only be dispatched if the status is Pending."
                );
            }

            foreach (var item in delivery.DeliveryDetails)
            {
                var inventory = await _unitOfWork.Inventory.GetByKey(
                    delivery.WarehouseId,
                    item.ProductId
                );
                if (inventory == null || inventory.Quantity < item.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for product '{item.Product?.Name ?? "unknown"}'."
                    );
                }

                inventory.Quantity -= item.Quantity;
                await _unitOfWork.Inventory.Update(inventory);
            }

            delivery.Status = DeliveryStatus.Complete;
            await _unitOfWork.Delivery.Update(delivery);

            await _unitOfWork.SaveChanges();

            return _mapper.Map<DeliveryRes>(delivery);
        }

        public async Task<bool> Delete(long id)
        {
            var delivery = await _unitOfWork.Delivery.GetById(id);
            if (delivery == null)
            {
                return false;
            }
            if (delivery.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException($"Order '{id}' has already been cancelled.");
            }

            DateOnly snapshotDate = (DateOnly)
                await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (today <= snapshotDate)
            {
                throw new InvalidOperationException(
                    $"Cannot cancel the delivery because the inventory has been finalized for this period."
                );
            }

            switch (delivery.Status)
            {
                case DeliveryStatus.Pending:
                    break;
                case DeliveryStatus.Complete:
                    var deliveryDetails = delivery.DeliveryDetails;
                    if (deliveryDetails == null || !deliveryDetails.Any())
                    {
                        throw new InvalidOperationException(
                            $"Delivery '{id}' is complete but has no details to revert stock."
                        );
                    }

                    foreach (var detail in deliveryDetails)
                    {
                        var inventory = await _unitOfWork.Inventory.GetByKey(
                            delivery.WarehouseId,
                            detail.ProductId
                        );
                        if (inventory == null)
                        {
                            throw new InvalidOperationException(
                                $"Inventory record not found for Product ID '{detail.ProductId}' in Warehouse ID '{delivery.WarehouseId}'."
                            );
                        }

                        inventory.Quantity = inventory.Quantity + detail.Quantity;
                        await _unitOfWork.Inventory.Update(inventory);
                    }
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Cannot cancel an order with the status '{delivery.Status}'."
                    );
            }

            delivery.Status = OrderStatus.Cancelled;
            delivery.IsActivated = false;

            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<List<DeliveryRes>> GetAllByMyself()
        {
            var curAccount = _currentUserService.GetUserAccount();
            var deliveries = await _unitOfWork.Delivery.GetByCreator(curAccount);
            return _mapper.Map<List<DeliveryRes>>(deliveries);
        }
    }
}
