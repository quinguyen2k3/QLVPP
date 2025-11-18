using AutoMapper;
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
            stockTake.WarehouseId = _currentUserService.GetWarehouseId();

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
            var stockTake = await _unitOfWork.StockTake.GetById(currentWarehouse);
            return _mapper.Map<List<StockTakeRes>>(stockTake);
        }

        public async Task<StockTakeRes?> Update(long id, StockTakeReq request)
        {
            var stockTake = await _unitOfWork.StockTake.GetById(id);

            if (stockTake == null)
                throw new KeyNotFoundException("The stock take record does not exist.");

            if (stockTake.WarehouseId != _currentUserService.GetWarehouseId())
                throw new UnauthorizedAccessException(
                    "You do not have permission to edit this stock take record."
                );

            _mapper.Map(request, stockTake);

            await _unitOfWork.StockTake.Update(stockTake);
            await _unitOfWork.SaveChanges();

            var response = _mapper.Map<StockTakeRes>(stockTake);
            return response;
        }
    }
}
