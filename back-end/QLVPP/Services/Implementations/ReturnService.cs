using AutoMapper;
using QLVPP.Constants.Status;
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

        public ReturnService(
            IUnitOfWork uniOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = uniOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<List<ReturnRes>> GetByWarehouse()
        {
            var warehouseId = _currentUserService.GetWarehouseId();
            var returns = await _unitOfWork.Return.GetByWarehouseId(warehouseId);
            return _mapper.Map<List<ReturnRes>>(returns);
        }

        public async Task<ReturnRes?> GetById(long id)
        {
            var assetLoan = await _unitOfWork.Return.GetById(id);
            return assetLoan == null ? null : _mapper.Map<ReturnRes>(assetLoan);
        }

        public async Task<ReturnRes> Create(ReturnReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();
            if (latestSnapshotDate != null && request.ReturnDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot receive items on date ({request.ReturnDate:dd/MM/yyyy}) as it falls within a closed accounting period."
                );
            }

            var delivery = await _unitOfWork.StockOut.GetById(request.DeliveryId);
            if (delivery == null)
            {
                throw new InvalidOperationException(
                    $"Delivery with Id {request.DeliveryId} not found."
                );
            }

            foreach (var detailReq in request.Items)
            {
                var deliveryDetail = delivery.StockOutDetails.FirstOrDefault(d =>
                    d.ProductId == detailReq.ProductId
                );

                if (deliveryDetail == null)
                {
                    throw new InvalidOperationException(
                        $"Product with Id {detailReq.ProductId} not found in Delivery #{delivery.Id}."
                    );
                }

                var totalDelivered = deliveryDetail.Quantity;
                var totalReturned = await _unitOfWork.Return.GetTotalReturnedQuantity(
                    delivery.Id,
                    detailReq.ProductId
                );

                var remaining = totalDelivered - totalReturned;
                var totalToReturn = detailReq.ReturnedQuantity + detailReq.DamagedQuantity;

                if (totalToReturn > remaining)
                {
                    throw new InvalidOperationException(
                        $"Product {detailReq.ProductId}: remaining = {remaining}, attempted = {totalToReturn}"
                    );
                }
            }
            var returnNote = _mapper.Map<Return>(request);
            returnNote.Status = ReturnStatus.Pending;

            await _unitOfWork.Return.Add(returnNote);
            await _unitOfWork.SaveChanges();

            var response = _mapper.Map<ReturnRes>(returnNote);
            return response;
        }

        public async Task<ReturnRes?> Update(long id, ReturnReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();
            if (latestSnapshotDate != null && request.ReturnDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot receive items on date ({request.ReturnDate:dd/MM/yyyy}) as it falls within a closed accounting period."
                );
            }

            var returnNote = await _unitOfWork.Return.GetById(id);
            if (returnNote == null)
                return null;

            if (returnNote.Status != ReturnStatus.Pending)
                throw new InvalidOperationException("Only pending return notes can be updated.");

            var delivery = await _unitOfWork.StockOut.GetById(request.DeliveryId);
            if (delivery == null)
                throw new InvalidOperationException(
                    $"Delivery with Id {request.DeliveryId} not found."
                );

            foreach (var item in request.Items)
            {
                var deliveryDetail = delivery.StockOutDetails.FirstOrDefault(d =>
                    d.ProductId == item.ProductId
                );
                if (deliveryDetail == null)
                    throw new InvalidOperationException(
                        $"Product {item.ProductId} not found in Delivery #{delivery.Id}."
                    );

                var totalDelivered = deliveryDetail.Quantity;
                var totalReturned = await _unitOfWork.Return.GetTotalReturnedQuantity(
                    delivery.Id,
                    item.ProductId
                );
                var remaining = totalDelivered - totalReturned;
                var totalToReturn = item.ReturnedQuantity + item.DamagedQuantity;

                if (totalToReturn > remaining)
                {
                    throw new InvalidOperationException(
                        $"Product {item.ProductId}: remaining = {remaining}, attempted = {totalToReturn}"
                    );
                }
            }
            _mapper.Map(request, returnNote);

            await _unitOfWork.Return.Update(returnNote);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<ReturnRes>(returnNote);
        }

        public async Task<ReturnRes?> Returned(long id, ReturnReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();
            if (latestSnapshotDate != null && request.ReturnDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot receive items on date ({request.ReturnDate:dd/MM/yyyy}) as it falls within a closed accounting period."
                );
            }

            var returnNote = await _unitOfWork.Return.GetById(id);
            if (returnNote == null)
                return null;

            if (returnNote.Status != ReturnStatus.Pending)
                throw new InvalidOperationException("Only pending return notes can be completed.");

            var delivery = await _unitOfWork.StockOut.GetById(returnNote.DeliveryId);
            if (delivery == null)
                throw new InvalidOperationException(
                    $"Delivery #{returnNote.DeliveryId} not found."
                );
            foreach (var detail in returnNote.ReturnDetails)
            {
                var deliveryDetail = delivery.StockOutDetails.FirstOrDefault(d =>
                    d.Id == detail.ProductId
                );
                if (deliveryDetail == null)
                    throw new InvalidOperationException(
                        $"Delivery detail not found for Product {detail.ProductId}."
                    );
                if (detail.ReturnedQuantity > 0)
                {
                    var inventory = await _unitOfWork.Inventory.GetByKey(
                        delivery.WarehouseId,
                        detail.ProductId
                    );
                    if (inventory == null)
                        throw new InvalidOperationException(
                            $"Inventory not found for Product {detail.ProductId}."
                        );

                    inventory.Quantity += detail.ReturnedQuantity;
                    await _unitOfWork.Inventory.Update(inventory);
                }
            }

            returnNote.Status = ReturnStatus.Complete;

            await _unitOfWork.Return.Update(returnNote);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<ReturnRes>(returnNote);
        }

        public async Task<bool> Delete(long id)
        {
            var returnNote = await _unitOfWork.Return.GetById(id);
            if (returnNote == null)
            {
                return false;
            }

            if (returnNote.Status == ReturnStatus.Cancelled)
            {
                throw new InvalidOperationException($"Return '{id}' has already been cancelled.");
            }

            DateOnly snapshotDate = (DateOnly)
                await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (today <= snapshotDate)
            {
                throw new InvalidOperationException(
                    $"Cannot cancel the return because the inventory has been finalized for this period."
                );
            }

            switch (returnNote.Status)
            {
                case StockInStatus.Pending:
                    break;

                case StockInStatus.Approve:
                    var returnDetails = returnNote.ReturnDetails;
                    if (returnDetails == null || !returnDetails.Any())
                    {
                        throw new InvalidOperationException(
                            $"Order '{id}' is complete but has no details to revert stock."
                        );
                    }

                    foreach (var detail in returnDetails)
                    {
                        var inventory = await _unitOfWork.Inventory.GetByKey(
                            returnNote.WarehouseId,
                            detail.ProductId
                        );
                        if (inventory == null)
                        {
                            throw new InvalidOperationException(
                                $"Inventory record not found for Product ID '{detail.ProductId}' in Warehouse ID '{returnNote.WarehouseId}'."
                            );
                        }

                        inventory.Quantity = inventory.Quantity - detail.ReturnedQuantity;
                        await _unitOfWork.Inventory.Update(inventory);
                    }
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Cannot cancel an order with the status '{returnNote.Status}'."
                    );
            }

            returnNote.Status = ReturnStatus.Cancelled;
            returnNote.IsActivated = false;

            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<List<ReturnRes>> GetAllByMyself()
        {
            var curAccount = _currentUserService.GetUserAccount();
            var returns = await _unitOfWork.Return.GetByCreator(curAccount);
            return _mapper.Map<List<ReturnRes>>(returns);
        }
    }
}
