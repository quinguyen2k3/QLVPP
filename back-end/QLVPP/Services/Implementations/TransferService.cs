using AutoMapper;
using QLVPP.Constants.Status;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class TransferService : ITransferService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public TransferService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<TransferRes> Approve(long id, TransferReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (latestSnapshotDate != null && request.TransferredDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create order note on date ({request.TransferredDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }

            var transfer = await _unitOfWork.Transfer.GetById(id);
            if (transfer == null)
            {
                throw new KeyNotFoundException($"Transfer with ID {id} not found.");
            }

            if (transfer.Status != TransferStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Cannot approve. Transfer must be in '{TransferStatus.Pending}' status."
                );
            }

            foreach (var detail in transfer.TransferDetail)
            {
                var inventory = await _unitOfWork.Inventory.GetByKey(
                    transfer.FromWarehouseId,
                    detail.ProductId
                );

                if (inventory == null || inventory.Quantity < detail.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Not enough stock for product ID {detail.ProductId} in warehouse {transfer.FromWarehouseId}. "
                            + $"Required: {detail.Quantity}, Available: {inventory?.Quantity ?? 0}"
                    );
                }

                inventory.Quantity -= detail.Quantity;
                await _unitOfWork.Inventory.Update(inventory);
            }
            var currentUserId = _currentUserService.GetUserId();

            transfer.Status = TransferStatus.Approved;
            transfer.ApproveDate = DateTime.Now;
            transfer.ApproverId = currentUserId;

            await _unitOfWork.Transfer.Update(transfer);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<TransferRes>(transfer);
        }

        public async Task<TransferRes> Create(TransferReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (latestSnapshotDate != null && request.TransferredDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create order note on date ({request.TransferredDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }

            var currentId = _currentUserService.GetUserId();

            var transfer = _mapper.Map<Transfer>(request);
            transfer.Status = TransferStatus.Pending;
            transfer.RequesterId = currentId;

            await _unitOfWork.Transfer.Add(transfer);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<TransferRes>(transfer);
        }

        public async Task<List<TransferRes>> GetAllByMyself()
        {
            var currentAccount = _currentUserService.GetUserAccount();
            var transfers = await _unitOfWork.Transfer.GetByCreator(currentAccount);
            return _mapper.Map<List<TransferRes>>(transfers);
        }

        public async Task<TransferRes?> GetById(long id)
        {
            var transfer = await _unitOfWork.Transfer.GetById(id);
            return transfer == null ? null : _mapper.Map<TransferRes>(transfer);
        }

        public async Task<List<TransferRes>> GetPendingFromWarehouse()
        {
            var currentWarehouse = _currentUserService.GetWarehouseId();
            var transfers = await _unitOfWork.Transfer.GetPendingByFromWarehouse(currentWarehouse);
            return _mapper.Map<List<TransferRes>>(transfers);
        }

        public async Task<List<TransferRes>> GetApprovedForWarehouse()
        {
            var currentWarehouse = _currentUserService.GetWarehouseId();
            var transfers = await _unitOfWork.Transfer.GetApprovedForWarehouse(currentWarehouse);
            return _mapper.Map<List<TransferRes>>(transfers);
        }

        public async Task<TransferRes> Receive(long id, TransferReq request)
        {
            var latestSnapshotDate = await _unitOfWork.InventorySnapshot.GetLatestSnapshotDate();

            if (latestSnapshotDate != null && request.TransferredDate <= latestSnapshotDate.Value)
            {
                throw new InvalidOperationException(
                    $"Cannot create order note on date ({request.TransferredDate:dd/MM/yyyy}) because it is within or before the last closed period ending on ({latestSnapshotDate.Value:dd/MM/yyyy})."
                );
            }

            var transfer = await _unitOfWork.Transfer.GetById(id);
            if (transfer == null)
            {
                throw new KeyNotFoundException($"Transfer with ID {id} not found.");
            }

            if (transfer.Status != TransferStatus.Pending)
            {
                throw new InvalidOperationException(
                    $"Cannot approve. Transfer must be in '{TransferStatus.Pending}' status."
                );
            }

            foreach (var detail in transfer.TransferDetail)
            {
                var inventory = await _unitOfWork.Inventory.GetByKey(
                    transfer.ToWarehouseId,
                    detail.ProductId
                );

                if (inventory == null || inventory.Quantity < detail.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Not enough stock for product ID {detail.ProductId} in warehouse {transfer.FromWarehouseId}. "
                            + $"Required: {detail.Quantity}, Available: {inventory?.Quantity ?? 0}"
                    );
                }

                inventory.Quantity += detail.Quantity;
                await _unitOfWork.Inventory.Update(inventory);
            }
            var currentUserId = _currentUserService.GetUserId();

            transfer.Status = TransferStatus.Received;
            transfer.ReceiverId = currentUserId;

            await _unitOfWork.Transfer.Update(transfer);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<TransferRes>(transfer);
        }

        public async Task<bool> Delete(long id)
        {
            var transfer = await _unitOfWork.Transfer.GetById(id);
            if (transfer == null)
            {
                return false;
            }

            if (transfer.Status == TransferStatus.Received)
            {
                throw new InvalidOperationException(
                    "Cannot cancel a completed transfer. Inventory has already been moved. A reverse transaction must be created."
                );
            }
            if (transfer.Status == TransferStatus.Cancelled)
            {
                return true;
            }

            transfer.IsActivated = false;
            transfer.Status = TransferStatus.Cancelled;

            await _unitOfWork.Transfer.Update(transfer);
            var savedChanges = await _unitOfWork.SaveChanges();

            return savedChanges > 0;
        }
    }
}
