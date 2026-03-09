using AutoMapper;
using QLVPP.Constants.Status;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.DTOs.Result;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public InventoryService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<List<InventorySnapshotRes>> GetListAsync(
            InventorySnapshotFilterReq filter
        )
        {
            var warehouseId = filter.WarehouseId ?? _currentUserService.GetWarehouseId();

            filter.WarehouseId = warehouseId;
            filter.OrderByDesc = true;

            var snapshots = await _unitOfWork.InventorySnapshot.GetListByConditionAsync(filter);

            return _mapper.Map<List<InventorySnapshotRes>>(snapshots);
        }

        public async Task<InventorySnapshotRes?> GetById(long id)
        {
            var snapshot = await _unitOfWork.InventorySnapshot.GetById(id);
            return snapshot == null ? null : _mapper.Map<InventorySnapshotRes>(snapshot);
        }

        public async Task<InventorySnapshotRes> Create(InventorySnapshotReq req)
        {
            var userAccount = _currentUserService.GetUserAccount();

            var existCondition = new InventorySnapshotFilterReq
            {
                WarehouseId = req.WarehouseId,
                ToDate = req.ToDate,
            };

            var isSnapshotExists = await _unitOfWork.InventorySnapshot.ExistsByConditionAsync(
                existCondition
            );

            if (isSnapshotExists)
                throw new Exception($"A snapshot for date {req.ToDate:dd/MM/yyyy} already exists.");

            var latestCondition = new InventorySnapshotFilterReq
            {
                WarehouseId = req.WarehouseId,
                OrderByDesc = true,
            };

            var prevSnapshot = await _unitOfWork.InventorySnapshot.GetOneByConditionAsync(
                latestCondition
            );

            if (prevSnapshot != null && req.FromDate <= prevSnapshot.ToDate)
            {
                throw new Exception(
                    $"FromDate ({req.FromDate:dd/MM/yyyy}) must be after the last snapshot date ({prevSnapshot.ToDate:dd/MM/yyyy})."
                );
            }

            var newSnapshotId = await _unitOfWork.InventorySnapshot.CreateSnapshotProcedure(
                req.WarehouseId,
                req.FromDate,
                req.ToDate,
                userAccount
            );

            if (newSnapshotId <= 0)
                throw new Exception(
                    "An error occurred while creating the inventory snapshot via procedure."
                );

            var snapshot = await _unitOfWork.InventorySnapshot.GetById(newSnapshotId);
            return _mapper.Map<InventorySnapshotRes>(snapshot);
        }
    }
}
