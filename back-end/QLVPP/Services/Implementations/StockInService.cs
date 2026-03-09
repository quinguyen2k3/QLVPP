using AutoMapper;
using QLVPP.Constants.Status;
using QLVPP.Constants.Types;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class StockInService : IStockInService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public StockInService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<StockInRes> Create(StockInReq request)
        {
            await ValidateAccountingPeriod(request.StockInDate, request.WarehouseId, "create");

            ValidateStockInType(request);
            await ValidateDuplicateProducts(request);

            if (request.Items == null || !request.Items.Any())
            {
                throw new InvalidOperationException(
                    "Stock-in voucher must contain at least one item."
                );
            }

            var requestedProductIds = request
                .Items.Select(item => item.ProductId)
                .Distinct()
                .ToList();
            var existingProducts = await _unitOfWork.Product.GetByIds(requestedProductIds);

            if (existingProducts.Count() != requestedProductIds.Count)
            {
                var invalidProductIds = requestedProductIds.Except(
                    existingProducts.Select(p => p.Id)
                );
                throw new InvalidOperationException(
                    $"One or more products were not found: {string.Join(", ", invalidProductIds)}."
                );
            }

            var stockIn = _mapper.Map<StockIn>(request);
            stockIn.Status = StockInStatus.Pending;

            await _unitOfWork.StockIn.Add(stockIn);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<StockInRes>(stockIn);
        }

        public async Task<StockInRes?> Update(long id, StockInReq request)
        {
            var stockIn = await _unitOfWork.StockIn.GetById(id);
            if (stockIn == null)
                return null;

            if (stockIn.CreatedBy != _currentUserService.GetUserAccount())
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to update a record that you did not create."
                );
            }

            if (stockIn.WarehouseId != _currentUserService.GetWarehouseId())
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to update a record from another warehouse."
                );
            }

            if (stockIn.Status != StockInStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only vouchers in 'Pending' status can be modified."
                );
            }

            if (stockIn.Type != request.Type)
            {
                throw new InvalidOperationException(
                    "Changing the Stock-In type is not allowed. Please delete and create a new voucher instead."
                );
            }

            await ValidateAccountingPeriod(request.StockInDate, stockIn.WarehouseId, "update");
            ValidateStockInType(request);
            await ValidateDuplicateProducts(request);

            if (request.Items == null || !request.Items.Any())
            {
                throw new InvalidOperationException(
                    "Stock-in voucher must contain at least one item."
                );
            }

            var requestedProductIds = request
                .Items.Select(item => item.ProductId)
                .Distinct()
                .ToList();
            var existingProducts = await _unitOfWork.Product.GetByIds(requestedProductIds);

            if (existingProducts.Count() != requestedProductIds.Count)
            {
                var invalidProductIds = requestedProductIds.Except(
                    existingProducts.Select(p => p.Id)
                );
                throw new InvalidOperationException(
                    $"Invalid Product IDs: {string.Join(", ", invalidProductIds)}."
                );
            }

            _mapper.Map(request, stockIn);

            await _unitOfWork.StockIn.Update(stockIn);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<StockInRes>(stockIn);
        }

        public async Task<bool> Approve(long id)
        {
            var stockIn = await _unitOfWork.StockIn.GetById(id);
            if (stockIn == null)
                return false;

            if (stockIn.WarehouseId != _currentUserService.GetWarehouseId())
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to approve a Stock In from another warehouse."
                );
            }

            await ValidateAccountingPeriod(stockIn.StockInDate, stockIn.WarehouseId, "approve");

            if (stockIn.Status == StockInStatus.Approve)
            {
                throw new InvalidOperationException("Stock In has already been approved.");
            }

            if (stockIn.Status != StockInStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Cannot approve Stock In with status '{stockIn.Status}'."
                );
            }

            var stockInDetails = stockIn.StockInDetails;
            if (stockInDetails == null || !stockInDetails.Any())
            {
                throw new InvalidOperationException($"Stock In '{id}' has no items to process.");
            }

            foreach (var detail in stockInDetails)
            {
                var inventory = await _unitOfWork.Inventory.GetByKey(
                    stockIn.WarehouseId,
                    detail.ProductId
                );

                if (inventory == null)
                {
                    var newInventory = new Inventory
                    {
                        ProductId = detail.ProductId,
                        WarehouseId = stockIn.WarehouseId,
                        Quantity = detail.Quantity,
                    };
                    await _unitOfWork.Inventory.Add(newInventory);
                }
                else
                {
                    inventory.Quantity += detail.Quantity;
                    await _unitOfWork.Inventory.Update(inventory);
                }
            }

            if (
                stockIn.Type == StockInType.Adjustment
                && !string.IsNullOrWhiteSpace(stockIn.ReferenceId)
            )
            {
                var stockTake = await _unitOfWork.StockTake.GetByCode(stockIn.ReferenceId);
                if (stockTake != null && stockTake.Details != null)
                {
                    var processedProductIds = stockInDetails.Select(d => d.ProductId).ToList();

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

            stockIn.Status = StockInStatus.Approve;
            stockIn.ApproverId = _currentUserService.GetUserId();
            stockIn.ApprovedDate = DateTime.UtcNow.Date;

            await _unitOfWork.StockIn.Update(stockIn);
            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<StockInRes?> GetById(long id)
        {
            var stockIn = await _unitOfWork.StockIn.GetById(id);
            return stockIn == null ? null : _mapper.Map<StockInRes>(stockIn);
        }

        public async Task<bool> Delete(long id)
        {
            var stockIn = await _unitOfWork.StockIn.GetById(id);
            if (stockIn == null)
                return false;

            if (stockIn.Status != StockInStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Cannot delete StockIn '{id}' because it is not PENDING. Current status: {stockIn.Status}"
                );
            }

            stockIn.IsActivated = false;
            await _unitOfWork.SaveChanges();
            return true;
        }

        public async Task<bool> Cancel(long id)
        {
            var stockIn = await _unitOfWork.StockIn.GetById(id);
            if (stockIn == null)
                return false;

            if (stockIn.Status == StockInStatus.Cancelled)
                throw new InvalidOperationException($"Stock In '{id}' has already been cancelled.");

            if (stockIn.Status == StockInStatus.Approve)
                throw new InvalidOperationException(
                    $"Cannot cancel an APPROVED Stock In using this function because items are already in inventory. Please use 'Revert' function instead."
                );

            stockIn.Status = StockInStatus.Cancelled;
            await _unitOfWork.SaveChanges();
            return true;
        }

        private async Task ValidateDuplicateProducts(StockInReq request)
        {
            if (
                request.Type != StockInType.Adjustment
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
                    $"Cannot {actionName} voucher on ({transactionDate:yyyy-MM-dd}) "
                        + $"because the accounting period was closed on ({latestSnapshotDate.Value:yyyy-MM-dd})."
                );
            }
        }

        private void ValidateStockInType(StockInReq request)
        {
            switch (request.Type)
            {
                case StockInType.Purchase:
                    if (!request.SupplierId.HasValue)
                        throw new InvalidOperationException(
                            "Supplier is required for Purchase stock-in."
                        );
                    request.FromWarehouseId = null;
                    request.FromDepartmentId = null;
                    break;

                case StockInType.Transfer:
                    if (!request.FromWarehouseId.HasValue)
                        throw new InvalidOperationException(
                            "Source warehouse is required for Transfer stock-in."
                        );
                    if (request.FromWarehouseId == request.WarehouseId)
                        throw new InvalidOperationException(
                            "Source and Destination warehouses cannot be the same."
                        );
                    request.SupplierId = null;
                    request.FromDepartmentId = null;
                    break;

                case StockInType.Return:
                    if (!request.FromDepartmentId.HasValue)
                        throw new InvalidOperationException(
                            "Source department is required for Return stock-in."
                        );
                    request.SupplierId = null;
                    request.FromWarehouseId = null;
                    break;

                case StockInType.Adjustment:
                    if (string.IsNullOrWhiteSpace(request.Note))
                        throw new InvalidOperationException(
                            "A detailed Note is required for Adjustment stock-in."
                        );
                    request.SupplierId = null;
                    request.FromWarehouseId = null;
                    request.FromDepartmentId = null;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(request.Type),
                        "Invalid Stock-In Type provided."
                    );
            }
        }

        public async Task<List<StockInRes>> GetByConditions(StockInFilterReq filter)
        {
            var entities = await _unitOfWork.StockIn.GetByConditions(filter);
            return _mapper.Map<List<StockInRes>>(entities);
        }
    }
}
