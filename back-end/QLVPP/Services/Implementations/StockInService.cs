using AutoMapper;
using QLVPP.Constants.Status;
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
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (latestSnapshotDate != null && request.StockInDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create order note on date ({request.StockInDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }

            if (request.Items != null && request.Items.Any())
            {
                var requestedProductIds = request
                    .Items.Select(item => item.ProductId)
                    .Distinct()
                    .ToList();

                var existingProducts = await _unitOfWork.Product.GetByIds(requestedProductIds);

                if (existingProducts.Count() != requestedProductIds.Count)
                {
                    var existingProductIds = existingProducts.Select(p => p.Id).ToHashSet();
                    var invalidProductIds = requestedProductIds.Where(id =>
                        !existingProductIds.Contains(id)
                    );

                    throw new InvalidOperationException(
                        $"One or more products do not exist. Invalid Product IDs: {string.Join(", ", invalidProductIds)}."
                    );
                }
            }
            var stockIn = _mapper.Map<StockIn>(request);
            stockIn.Status = StockInStatus.Pending;
            await _unitOfWork.StockIn.Add(stockIn);

            await _unitOfWork.SaveChanges();

            var response = _mapper.Map<StockInRes>(stockIn);
            return response;
        }

        public async Task<List<StockInRes>> GetPendingByWarehouse()
        {
            var warehouseId = _currentUserService.GetWarehouseId();
            var requisitions = await _unitOfWork.StockIn.GetPendingByWarehouseId(warehouseId);
            return _mapper.Map<List<StockInRes>>(requisitions);
        }

        public async Task<StockInRes?> GetById(long id)
        {
            var stockIn = await _unitOfWork.StockIn.GetById(id);
            return stockIn == null ? null : _mapper.Map<StockInRes>(stockIn);
        }

        public async Task<StockInRes?> Update(long id, StockInReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (latestSnapshotDate != null && request.StockInDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create order note on date ({request.StockInDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }

            if (request.Items != null && request.Items.Any())
            {
                var requestedProductIds = request
                    .Items.Select(item => item.ProductId)
                    .Distinct()
                    .ToList();

                var existingProducts = await _unitOfWork.Product.GetByIds(requestedProductIds);

                if (existingProducts.Count() != requestedProductIds.Count)
                {
                    var existingProductIds = existingProducts.Select(p => p.Id).ToHashSet();
                    var invalidProductIds = requestedProductIds.Where(id =>
                        !existingProductIds.Contains(id)
                    );

                    throw new InvalidOperationException(
                        $"One or more products do not exist. Invalid Product IDs: {string.Join(", ", invalidProductIds)}."
                    );
                }
            }

            var stockIn = await _unitOfWork.StockIn.GetById(id);
            if (stockIn == null)
                return null;

            if (stockIn.Status != StockInStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Stock In is not in Pending status and cannot be updated."
                );
            }

            _mapper.Map(request, stockIn);

            await _unitOfWork.StockIn.Update(stockIn);

            await _unitOfWork.SaveChanges();

            return _mapper.Map<StockInRes>(stockIn);
        }

        public async Task<bool> Delete(long id)
        {
            var stockIn = await _unitOfWork.StockIn.GetById(id);
            if (stockIn == null)
            {
                return false;
            }

            if (stockIn.Status == StockInStatus.Cancelled)
            {
                throw new InvalidOperationException($"Stock In '{id}' has already been cancelled.");
            }

            DateOnly snapshotDate = (DateOnly)
                await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (stockIn.StockInDate <= snapshotDate)
            {
                throw new InvalidOperationException(
                    $"Cannot cancel the order because the inventory has been finalized for this period."
                );
            }

            switch (stockIn.Status)
            {
                case StockInStatus.Pending:
                    break;

                case StockInStatus.Approve:
                    var stockInDetails = stockIn.StockInDetails;
                    if (stockInDetails == null || !stockInDetails.Any())
                    {
                        throw new InvalidOperationException(
                            $"Order '{id}' is complete but has no details to revert stock."
                        );
                    }

                    foreach (var detail in stockInDetails)
                    {
                        var inventory = await _unitOfWork.Inventory.GetByKey(
                            stockIn.WarehouseId,
                            detail.ProductId
                        );
                        if (inventory == null)
                        {
                            throw new InvalidOperationException(
                                $"Inventory record not found for Product ID '{detail.ProductId}' in Warehouse ID '{stockIn.WarehouseId}'."
                            );
                        }

                        inventory.Quantity = inventory.Quantity - detail.Quantity;
                        await _unitOfWork.Inventory.Update(inventory);
                    }
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Cannot cancel an stockIn with the status '{stockIn.Status}'."
                    );
            }

            stockIn.Status = StockInStatus.Cancelled;
            stockIn.IsActivated = false;

            await _unitOfWork.SaveChanges();

            return true;
        }

        public async Task<StockInRes?> Approve(long id, StockInReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();
            var stockIn = await _unitOfWork.StockIn.GetById(id);
            if (stockIn == null)
                return null;

            if (latestSnapshotDate != null && stockIn.StockInDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot process Stock In on date ({stockIn.StockInDate:dd/MM/yyyy}) as it falls within a closed accounting period."
                );
            }

            if (stockIn.Status == StockInStatus.Approve)
            {
                throw new InvalidOperationException("Stock In has already been completed.");
            }

            foreach (var detail in stockIn.StockInDetails)
            {
                var inventory = await _unitOfWork.Inventory.GetByKey(
                    stockIn.WarehouseId,
                    detail.ProductId
                );
                if (inventory == null)
                {
                    throw new InvalidOperationException(
                        $"Inventory record not found for Product ID '{detail.ProductId}' in Warehouse ID '{stockIn.WarehouseId}'."
                    );
                }

                inventory.Quantity += detail.Quantity;
                await _unitOfWork.Inventory.Update(inventory);
            }

            stockIn.Status = StockInStatus.Approve;
            stockIn.ApproverId = _currentUserService.GetUserId();
            stockIn.ApprovedDate = DateTime.Now;

            await _unitOfWork.StockIn.Update(stockIn);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<StockInRes>(stockIn);
        }

        public async Task<List<StockInRes>> GetAllByMyself()
        {
            var curAccount = _currentUserService.GetUserAccount();
            var requisitions = await _unitOfWork.StockIn.GetByCreator(curAccount);
            return _mapper.Map<List<StockInRes>>(requisitions);
        }

        public async Task<List<StockInRes>> GetByWarehouse()
        {
            var curWarehouse = _currentUserService.GetWarehouseId();
            var requisitions = await _unitOfWork.StockIn.GetByWarehouseId(curWarehouse);
            return _mapper.Map<List<StockInRes>>(requisitions);
        }
    }
}
