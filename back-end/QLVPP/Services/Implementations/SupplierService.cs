using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private const string CacheKey_GetAll = "suppliers:all";
        private const string CacheKey_GetAllActivated = "suppliers:activated";

        private string CacheKey_GetById(long id) => $"suppliers:{id}";

        public SupplierService(IUnitOfWork unitOfWord, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWord;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<SupplierRes> Create(SupplierReq request)
        {
            var supplier = _mapper.Map<Supplier>(request);

            await _unitOfWork.Supplier.Add(supplier);
            await _unitOfWork.SaveChanges();

            await ClearCaches();

            return _mapper.Map<SupplierRes>(supplier);
        }

        public async Task<List<SupplierRes>> GetAll()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAll,
                async () =>
                {
                    var suppliers = await _unitOfWork.Supplier.GetAll();
                    return _mapper.Map<List<SupplierRes>>(suppliers);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<List<SupplierRes>> GetAllActivated()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAllActivated,
                async () =>
                {
                    var suppliers = await _unitOfWork.Supplier.GetAllIsActivated();
                    return _mapper.Map<List<SupplierRes>>(suppliers);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<SupplierRes?> GetById(long id)
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetById(id),
                async () =>
                {
                    var supplier = await _unitOfWork.Supplier.GetById(id);
                    return supplier == null ? null : _mapper.Map<SupplierRes>(supplier);
                },
                TimeSpan.FromMinutes(30)
            );
        }

        public async Task<SupplierRes?> Update(long id, SupplierReq request)
        {
            var supplier = await _unitOfWork.Supplier.GetById(id);
            if (supplier == null)
                return null;

            _mapper.Map(request, supplier);

            await _unitOfWork.Supplier.Update(supplier);
            await _unitOfWork.SaveChanges();

            await ClearCaches();

            return _mapper.Map<SupplierRes>(supplier);
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
