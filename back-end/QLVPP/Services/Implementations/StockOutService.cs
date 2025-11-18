using AutoMapper;
using QLVPP.Constants.Status;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class StockOutService : IStockOutService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public StockOutService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<StockOutRes> Create(StockOutReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (latestSnapshotDate != null && request.StockOutDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create Stock out note on date ({request.StockOutDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }
            var stockOut = _mapper.Map<StockOut>(request);
            stockOut.Status = StockOutStatus.Pending;

            await _unitOfWork.StockOut.Add(stockOut);
            await _unitOfWork.SaveChanges();

            var response = _mapper.Map<StockOutRes>(stockOut);
            return response;
        }

        public async Task<List<StockOutRes>> GetPendingByWarehouse()
        {
            var warehouseId = _currentUserService.GetWarehouseId();
            var deliveries = await _unitOfWork.StockOut.GetPendingByWarehouseId(warehouseId);
            return _mapper.Map<List<StockOutRes>>(deliveries);
        }

        public async Task<StockOutRes?> GetById(long id)
        {
            var stockOut = await _unitOfWork.StockOut.GetById(id);
            return stockOut == null ? null : _mapper.Map<StockOutRes>(stockOut);
        }

        public async Task<StockOutRes?> Update(long id, StockOutReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (latestSnapshotDate != null && request.StockOutDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create stock out note on date ({request.StockOutDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }

            var stockOut = await _unitOfWork.StockOut.GetById(id);
            if (stockOut == null)
                return null;

            if (stockOut.WarehouseId != _currentUserService.GetWarehouseId())
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to update or approve a record from another warehouse."
                );
            }

            if (stockOut.Status != StockOutStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Stock out is not in Pending status and cannot be updated."
                );
            }

            _mapper.Map(request, stockOut);

            await _unitOfWork.StockOut.Update(stockOut);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<StockOutRes>(stockOut);
        }

        public async Task<StockOutRes?> Approve(long id, StockOutReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();
            if (latestSnapshotDate != null && request.StockOutDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create stock out note on date ({request.StockOutDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }

            var stockOut = await _unitOfWork.StockOut.GetById(id);
            if (stockOut == null)
            {
                return null;
            }

            if (stockOut.WarehouseId != _currentUserService.GetWarehouseId())
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to update or approve a record from another warehouse."
                );
            }

            if (stockOut.Status != StockOutStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Stock out can only be dispatched if the status is Pending."
                );
            }

            foreach (var item in stockOut.StockOutDetails)
            {
                var inventory = await _unitOfWork.Inventory.GetByKey(
                    stockOut.WarehouseId,
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

            stockOut.Status = StockOutStatus.Approved;
            stockOut.ApproverId = _currentUserService.GetUserId();
            stockOut.ApprovedDate = DateTime.Now;

            await _unitOfWork.StockOut.Update(stockOut);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<StockOutRes>(stockOut);
        }

        public async Task<StockOutRes?> Receive(long id)
        {
            var stockOut = await _unitOfWork.StockOut.GetById(id);
            if (stockOut == null)
                return null;

            if (stockOut.Status != StockOutStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Stock out can only be received if it has been approved."
                );
            }

            stockOut.ReceiverId = _currentUserService.GetUserId();
            stockOut.Status = StockOutStatus.Received;

            await _unitOfWork.StockOut.Update(stockOut);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<StockOutRes>(stockOut);
        }

        public async Task<bool> Delete(long id)
        {
            var stockOut = await _unitOfWork.StockOut.GetById(id);
            if (stockOut == null)
            {
                return false;
            }
            if (stockOut.Status == StockInStatus.Cancelled)
            {
                throw new InvalidOperationException($"Order '{id}' has already been cancelled.");
            }

            DateOnly snapshotDate = (DateOnly)
                await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (today <= snapshotDate)
            {
                throw new InvalidOperationException(
                    $"Cannot cancel the stock out because the inventory has been finalized for this period."
                );
            }

            switch (stockOut.Status)
            {
                case StockOutStatus.Pending:
                    break;
                case StockOutStatus.Approved:
                    var stockOutDetails = stockOut.StockOutDetails;
                    if (stockOutDetails == null || !stockOutDetails.Any())
                    {
                        throw new InvalidOperationException(
                            $"stockOut '{id}' is complete but has no details to revert stock."
                        );
                    }

                    foreach (var detail in stockOutDetails)
                    {
                        var inventory = await _unitOfWork.Inventory.GetByKey(
                            stockOut.WarehouseId,
                            detail.ProductId
                        );
                        if (inventory == null)
                        {
                            throw new InvalidOperationException(
                                $"Inventory record not found for Product ID '{detail.ProductId}' in Warehouse ID '{stockOut.WarehouseId}'."
                            );
                        }

                        inventory.Quantity = inventory.Quantity + detail.Quantity;
                        await _unitOfWork.Inventory.Update(inventory);
                    }
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Cannot cancel an order with the status '{stockOut.Status}'."
                    );
            }

            stockOut.Status = StockInStatus.Cancelled;
            stockOut.IsActivated = false;

            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<List<StockOutRes>> GetAllByMyself()
        {
            var curAccount = _currentUserService.GetUserAccount();
            var deliveries = await _unitOfWork.StockOut.GetByCreator(curAccount);
            return _mapper.Map<List<StockOutRes>>(deliveries);
        }

        public async Task<List<StockOutRes>> GetApprovedForDepartment()
        {
            var curDepartment = _currentUserService.GetDepartmentId();
            var deliveries = await _unitOfWork.StockOut.GetApprovedByDepartmentId(curDepartment);
            return _mapper.Map<List<StockOutRes>>(deliveries);
        }

        public async Task<List<StockOutRes>> GetByWarehouse()
        {
            var curWarehouse = _currentUserService.GetWarehouseId();
            var deliveries = await _unitOfWork.StockOut.GetByWarehouseId(curWarehouse);
            return _mapper.Map<List<StockOutRes>>(deliveries);
        }
    }
}
