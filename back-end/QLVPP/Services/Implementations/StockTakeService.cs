using AutoMapper;
using QLVPP.Constants.Status;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class StockTakeService : IStockTakeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public StockTakeService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<StockTakeRes> Create(StockTakeReq request)
        {
            var stockTake = _mapper.Map<StockTake>(request);

            foreach (var detail in stockTake.Details)
            {
                var currentInventory = await _unitOfWork.Inventory.GetByKey(
                    stockTake.WarehouseId,
                    detail.ProductId
                );
                if (currentInventory == null)
                {
                    throw new InvalidOperationException(
                        $"Product with ID {detail.ProductId} does not exist in the specified warehouse."
                    );
                }
                detail.SysQty = currentInventory.Quantity;
            }

            await _unitOfWork.StockTake.Add(stockTake);
            await _unitOfWork.SaveChanges();

            var response = _mapper.Map<StockTakeRes>(stockTake);
            return response;
        }

        public async Task<StockTakeRes?> GetById(long id)
        {
            var stockTake = await _unitOfWork.StockTake.GetById(id);
            return stockTake == null ? null : _mapper.Map<StockTakeRes>(stockTake);
        }

        public async Task<List<StockTakeRes>> GetByWarehouse()
        {
            var currentWarehouse = _currentUserService.GetWarehouseId();
            var stockTake = await _unitOfWork.StockTake.GetByWarehouseId(currentWarehouse);
            return _mapper.Map<List<StockTakeRes>>(stockTake);
        }

        public async Task<StockTakeRes?> Update(long id, StockTakeReq request)
        {
            var stockTake = await _unitOfWork.StockTake.GetById(id);

            if (stockTake == null)
                throw new KeyNotFoundException("The stock take record does not exist.");

            if (stockTake.CreatedBy != _currentUserService.GetUserAccount())
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to update a record that you did not create."
                );
            }

            if (stockTake.WarehouseId != _currentUserService.GetWarehouseId())
                throw new UnauthorizedAccessException(
                    "You do not have permission to edit this stock take record."
                );

            _mapper.Map(request, stockTake);
            foreach (var detail in stockTake.Details)
            {
                var currentInventory = await _unitOfWork.Inventory.GetByKey(
                    stockTake.WarehouseId,
                    detail.ProductId
                );
                if (currentInventory == null)
                {
                    throw new InvalidOperationException(
                        $"Product with ID {detail.ProductId} does not exist in the specified warehouse."
                    );
                }

                detail.SysQty = currentInventory.Quantity;
            }

            await _unitOfWork.StockTake.Update(stockTake);
            await _unitOfWork.SaveChanges();

            var response = _mapper.Map<StockTakeRes>(stockTake);
            return response;
        }

        public async Task<bool> Approve(long id)
        {
            var stockTake = await _unitOfWork.StockTake.GetById(id);

            if (stockTake == null)
                throw new KeyNotFoundException("Stock take record does not exist.");

            if (stockTake.Status != StockTakeStatus.Pending)
                throw new InvalidOperationException(
                    "Only stock take records in Pending status can be approved."
                );

            stockTake.Status = StockTakeStatus.Approve;

            await _unitOfWork.StockTake.Update(stockTake);
            return await _unitOfWork.SaveChanges() > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var stockTake = await _unitOfWork.StockTake.GetById(id);

            if (stockTake == null)
                throw new KeyNotFoundException("Stock take record does not exist.");

            if (stockTake.Status != StockTakeStatus.Pending)
                throw new InvalidOperationException(
                    "Only stock take records in Pending status can be deleted."
                );

            stockTake.IsActivated = false;

            await _unitOfWork.StockTake.Update(stockTake);
            return await _unitOfWork.SaveChanges() > 0;
        }

        public async Task<bool> Cancel(long id)
        {
            var stockTake = await _unitOfWork.StockTake.GetById(id);

            if (stockTake == null)
                throw new KeyNotFoundException("Stock take record does not exist.");

            if (stockTake.Status != StockTakeStatus.Pending)
                throw new InvalidOperationException(
                    "Only stock take records in Pending status can be cancelled."
                );

            if (stockTake.WarehouseId != _currentUserService.GetWarehouseId())
                throw new UnauthorizedAccessException(
                    "You do not have permission to operate on this warehouse."
                );

            stockTake.Status = StockTakeStatus.Cancelled;

            await _unitOfWork.StockTake.Update(stockTake);
            return await _unitOfWork.SaveChanges() > 0;
        }
    }
}
