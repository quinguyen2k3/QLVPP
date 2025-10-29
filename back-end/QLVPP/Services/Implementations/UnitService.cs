using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class UnitService : IUnitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        private const string CacheKey_GetAll = "units:all";
        private const string CacheKey_GetAllActivated = "units:activated";

        private string CacheKey_GetById(long id) => $"units:{id}";

        public UnitService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<UnitRes> Create(UnitReq request)
        {
            var unit = _mapper.Map<Unit>(request);

            await _unitOfWork.Unit.Add(unit);
            await _unitOfWork.SaveChanges();

            await ClearCaches();

            return _mapper.Map<UnitRes>(unit);
        }

        public async Task<List<UnitRes>> GetAll()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAll,
                async () =>
                {
                    var units = await _unitOfWork.Unit.GetAll();
                    return _mapper.Map<List<UnitRes>>(units);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<List<UnitRes>> GetAllActivated()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAllActivated,
                async () =>
                {
                    var units = await _unitOfWork.Unit.GetAllIsActivated();
                    return _mapper.Map<List<UnitRes>>(units);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<UnitRes?> GetById(long id)
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetById(id),
                async () =>
                {
                    var unit = await _unitOfWork.Unit.GetById(id);
                    return unit == null ? null : _mapper.Map<UnitRes>(unit);
                },
                TimeSpan.FromMinutes(30)
            );
        }

        public async Task<UnitRes?> Update(long id, UnitReq request)
        {
            var unit = await _unitOfWork.Unit.GetById(id);
            if (unit == null)
                return null;

            _mapper.Map(request, unit);

            await _unitOfWork.Unit.Update(unit);
            await _unitOfWork.SaveChanges();

            await ClearCaches();

            return _mapper.Map<UnitRes>(unit);
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
