using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private const string CacheKey_GetAll = "warehouses:all";
        private const string CacheKey_GetAllActivated = "warehouses:activated";

        private string CacheKey_GetById(long id) => $"warehouses:{id}";

        public WarehouseService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<WarehouseRes> Create(WarehouseReq request)
        {
            var warehouse = _mapper.Map<Warehouse>(request);

            await _unitOfWork.Warehouse.Add(warehouse);
            await _unitOfWork.SaveChanges();

            await ClearCaches();

            return _mapper.Map<WarehouseRes>(warehouse);
        }

        public async Task<List<WarehouseRes>> GetAll()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAll,
                async () =>
                {
                    var warehouse = await _unitOfWork.Warehouse.GetAll();
                    return _mapper.Map<List<WarehouseRes>>(warehouse);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<List<WarehouseRes>> GetAllActivated()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAllActivated,
                async () =>
                {
                    var warehouse = await _unitOfWork.Warehouse.GetAllIsActivated();
                    return _mapper.Map<List<WarehouseRes>>(warehouse);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<WarehouseRes?> GetById(long id)
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetById(id),
                async () =>
                {
                    var warehouse = await _unitOfWork.Warehouse.GetById(id);
                    return warehouse == null ? null : _mapper.Map<WarehouseRes>(warehouse);
                },
                TimeSpan.FromMinutes(30)
            );
        }

        public async Task<WarehouseRes?> Update(long id, WarehouseReq request)
        {
            var warehouse = await _unitOfWork.Warehouse.GetById(id);
            if (warehouse == null)
                return null;

            _mapper.Map(request, warehouse);

            await _unitOfWork.Warehouse.Update(warehouse);
            await _unitOfWork.SaveChanges();

            await ClearCaches(id);

            return _mapper.Map<WarehouseRes>(warehouse);
        }

        private async Task ClearCaches(long? id = null)
        {
            await _cacheService.Remove(CacheKey_GetAll);
            await _cacheService.Remove(CacheKey_GetAllActivated);

            if (id.HasValue)
            {
                await _cacheService.Remove(CacheKey_GetById(id.Value));
            }
        }
    }
}
