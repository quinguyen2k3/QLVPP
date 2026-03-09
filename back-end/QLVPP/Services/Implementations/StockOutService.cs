using AutoMapper;
using QLVPP.Constants.Status;
using QLVPP.Constants.Types;
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

        public async Task<List<StockOutRes>> GetByConditions(StockOutFilterReq filter)
        {
            var entities = await _unitOfWork.StockOut.GetByConditions(filter);
            return _mapper.Map<List<StockOutRes>>(entities);
        }

        public async Task<StockOutRes?> GetById(long id)
        {
            var stockOut = await _unitOfWork.StockOut.GetById(id);
            return stockOut == null ? null : _mapper.Map<StockOutRes>(stockOut);
        }

        public async Task<StockOutRes> Create(StockOutReq request)
        {
            await ValidateAccountingPeriod(request.StockOutDate, request.WarehouseId, "create");
            ValidateStockOutType(request);
            await ValidateDuplicateProducts(request);

            var stockOut = _mapper.Map<StockOut>(request);
            stockOut.Status = StockOutStatus.Pending;

            await _unitOfWork.StockOut.Add(stockOut);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<StockOutRes>(stockOut);
        }

        public async Task<StockOutRes?> Update(long id, StockOutReq request)
        {
            var stockOut = await _unitOfWork.StockOut.GetById(id);
            if (stockOut == null)
                return null;

            if (stockOut.CreatedBy != _currentUserService.GetUserAccount())
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to update a record that you did not create."
                );
            }

            if (stockOut.WarehouseId != _currentUserService.GetWarehouseId())
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to update a record from another warehouse."
                );
            }

            if (stockOut.Status != StockOutStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Stock out is not in Pending status and cannot be updated."
                );
            }

            await ValidateAccountingPeriod(request.StockOutDate, stockOut.WarehouseId, "update");
            ValidateStockOutType(request);
            await ValidateDuplicateProducts(request);

            _mapper.Map(request, stockOut);

            await _unitOfWork.StockOut.Update(stockOut);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<StockOutRes>(stockOut);
        }

        public async Task<bool> Approve(long id)
        {
            var stockOut = await _unitOfWork.StockOut.GetById(id);
            if (stockOut == null)
                return false;

            if (stockOut.WarehouseId != _currentUserService.GetWarehouseId())
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to approve a record from another warehouse."
                );
            }

            await ValidateAccountingPeriod(stockOut.StockOutDate, stockOut.WarehouseId, "approve");

            if (stockOut.Status != StockOutStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Stock out can only be approved if the status is Pending."
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
                        $"Insufficient stock for product '{item.Product?.Name}'."
                    );
                }
                inventory.Quantity -= item.Quantity;
                await _unitOfWork.Inventory.Update(inventory);
            }

            if (
                stockOut.Type == StockOutType.Adjustment
                && !string.IsNullOrWhiteSpace(stockOut.ReferenceId)
            )
            {
                var stockTake = await _unitOfWork.StockTake.GetByCode(stockOut.ReferenceId);
                if (stockTake != null && stockTake.Details != null)
                {
                    var processedProductIds = stockOut
                        .StockOutDetails.Select(d => d.ProductId)
                        .ToList();

                    foreach (var detail in stockTake.Details)
                    {
                        if (processedProductIds.Contains(detail.ProductId))
                        {
                            detail.IsProcessed = true;
                        }
                    }

                    await _unitOfWork.StockTake.Update(stockTake);
                }
            }

            stockOut.Status = StockOutStatus.Approved;
            stockOut.ApproverId = _currentUserService.GetUserId();
            stockOut.ApprovedDate = DateTime.Now.Date;

            await _unitOfWork.StockOut.Update(stockOut);
            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> Receive(long id)
        {
            var stockOut = await _unitOfWork.StockOut.GetById(id);
            if (stockOut == null)
                return false;

            if (stockOut.Status != StockOutStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Stock out can only be received if it has been approved."
                );
            }

            stockOut.ReceiverId = _currentUserService.GetUserId();
            stockOut.ReceivedDate = DateTime.Now;
            stockOut.Status = StockOutStatus.Received;

            await _unitOfWork.StockOut.Update(stockOut);
            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> Cancel(long id)
        {
            var stockOut = await _unitOfWork.StockOut.GetById(id);
            if (stockOut == null)
                return false;

            if (stockOut.Status != StockOutStatus.Pending)
            {
                throw new InvalidOperationException("Only PENDING stock outs can be cancelled.");
            }

            stockOut.Status = StockOutStatus.Cancelled;
            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var stockOut = await _unitOfWork.StockOut.GetById(id);
            if (stockOut == null)
                return false;

            if (stockOut.Status != StockOutStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Cannot delete record because it is not PENDING."
                );
            }

            stockOut.IsActivated = false;
            await _unitOfWork.SaveChanges();

            return true;
        }

        private async Task ValidateDuplicateProducts(StockOutReq request)
        {
            if (
                request.Type != StockOutType.Adjustment
                || string.IsNullOrWhiteSpace(request.ReferenceId)
            )
                return;

            var stockTake = await _unitOfWork.StockTake.GetByCode(request.ReferenceId);

            if (stockTake == null || stockTake.Status != StockTakeStatus.Approve)
                return;

            if (stockTake.Details == null)
                return;

            var requestedProductIds = request.Items.Select(i => i.ProductId).ToList();

            var processedProductIds = stockTake
                .Details.Where(d => requestedProductIds.Contains(d.ProductId) && d.IsProcessed)
                .Select(d => d.ProductId)
                .ToList();

            if (processedProductIds.Any())
            {
                throw new InvalidOperationException(
                    $"One or more products ({string.Join(", ", processedProductIds)}) have already been processed in reference '{request.ReferenceId}'."
                );
            }
        }

        private async Task ValidateAccountingPeriod(
            DateOnly transactionDate,
            long? warehouseId,
            string actionName
        )
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestToDateAsync(
                warehouseId
            );

            if (latestSnapshotDate != null && transactionDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot {actionName} stock out on date ({transactionDate:dd/MM/yyyy}) "
                        + $"because it is within or before the last closed period ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }
        }

        private void ValidateStockOutType(StockOutReq request)
        {
            switch (request.Type)
            {
                case StockOutType.Usage:
                    if (!request.DepartmentId.HasValue)
                        throw new InvalidDataException(
                            "Department is required for Usage stock out."
                        );
                    request.ToWarehouseId = null;
                    break;

                case StockOutType.Transfer:
                    if (!request.ToWarehouseId.HasValue)
                        throw new InvalidDataException(
                            "ToWarehouse is required for Transfer stock out."
                        );
                    if (request.WarehouseId == request.ToWarehouseId)
                        throw new InvalidDataException(
                            "Source and Destination warehouse cannot be the same."
                        );
                    request.DepartmentId = null;
                    break;

                case StockOutType.Adjustment:
                    if (string.IsNullOrWhiteSpace(request.Note))
                        throw new InvalidDataException(
                            "Note is required for Adjustment stock out."
                        );
                    request.DepartmentId = null;
                    request.ToWarehouseId = null;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(request.Type),
                        "Invalid stock out type."
                    );
            }
        }
    }
}
