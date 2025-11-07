using AutoMapper;
using QLVPP.Constants.Status;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class InventorySnapshotService : IInventorySnapshotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public InventorySnapshotService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<InventorySnapshotRes> Create()
        {
            var warehouseId = _currentUserService.GetWarehouseId();

            var now = DateOnly.FromDateTime(DateTime.Now);
            var period = $"{now:yyyy-MM}";

            var isSnapshotExists = await _unitOfWork.InventorySnapshot.ExistsBySnapshotDate(
                now.Year,
                now.Month
            );
            if (isSnapshotExists)
                throw new Exception($"The {period} has been previously closed.");
            var prevSnapshot = await _unitOfWork.InventorySnapshot.GetLatestByWarehouseId(
                warehouseId
            );
            DateOnly startDate =
                prevSnapshot != null
                    ? prevSnapshot.SnapshotDate.AddDays(1)
                    : new DateOnly(2000, 1, 1);
            DateOnly endDate = now;

            var totalIn = await _unitOfWork.Product.GetTotalIn(warehouseId, startDate, endDate);
            var totalOut = await _unitOfWork.Product.GetTotalOut(warehouseId, startDate, endDate);
            var totalReturn = await _unitOfWork.Product.GetTotalReturnAsync(
                warehouseId,
                startDate,
                endDate
            );
            var allProducts = await _unitOfWork.Product.GetByWarehouseId(warehouseId);
            var openingBalances =
                prevSnapshot?.SnapshotDetails.ToDictionary(x => x.ProductId, x => x.ClosingQty)
                ?? new Dictionary<long, int>();

            var snapshotDetails = new List<SnapshotDetail>();
            foreach (var product in allProducts)
            {
                var productId = product.Id;
                var openingQty = openingBalances.GetValueOrDefault(productId);

                var inQty = totalIn.FirstOrDefault(x => x.ProductId == productId)?.Quantity ?? 0;
                var outQty = totalOut.FirstOrDefault(x => x.ProductId == productId)?.Quantity ?? 0;
                var returnQty =
                    totalReturn.FirstOrDefault(x => x.ProductId == productId)?.Quantity ?? 0;

                var closingQty = openingQty + inQty - outQty + returnQty;

                snapshotDetails.Add(
                    new SnapshotDetail
                    {
                        ProductId = productId,
                        OpeningQty = openingQty,
                        TotalIn = inQty,
                        TotalOut = outQty,
                        ClosingQty = closingQty,
                    }
                );
            }
            var snapshot = new InventorySnapshot
            {
                WarehouseId = warehouseId,
                SnapshotDate = endDate,
                Status = InventorySnapshotStatus.Pending,
                SnapshotDetails = snapshotDetails,
            };

            await _unitOfWork.InventorySnapshot.Add(snapshot);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<InventorySnapshotRes>(snapshot);
        }

        public async Task<InventorySnapshotRes?> GetById(long id)
        {
            var snapshot = await _unitOfWork.InventorySnapshot.GetById(id);
            return snapshot == null ? null : _mapper.Map<InventorySnapshotRes>(snapshot);
        }

        public async Task<List<InventorySnapshotRes>> GetByWarehouse()
        {
            var curWarehouseId = _currentUserService.GetWarehouseId();
            var snapshots = await _unitOfWork.InventorySnapshot.GetByWarehouseId(curWarehouseId);
            return _mapper.Map<List<InventorySnapshotRes>>(snapshots);
        }

        public async Task<InventorySnapshotRes?> Close(long id)
        {
            var snapshot =
                await _unitOfWork.InventorySnapshot.GetById(id)
                ?? throw new Exception("The snapshot to be locked was not found.");
            if (snapshot.Status == InventorySnapshotStatus.Complete)
                throw new Exception("This snapshot was previously locked.");

            snapshot.Status = InventorySnapshotStatus.Complete;
            await _unitOfWork.InventorySnapshot.Update(snapshot);
            await _unitOfWork.SaveChanges();
            return _mapper.Map<InventorySnapshotRes>(snapshot);
        }
    }
}
