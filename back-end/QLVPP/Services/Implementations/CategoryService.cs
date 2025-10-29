using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        private const string CacheKey_GetAll = "categories:all";
        private const string CacheKey_GetAllActivated = "categories:activated";

        private string CacheKey_GetById(long id) => $"categories:{id}";

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<List<CategoryRes>> GetAll()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAll,
                async () =>
                {
                    var categories = await _unitOfWork.Category.GetAll();
                    return _mapper.Map<List<CategoryRes>>(categories);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<List<CategoryRes>> GetAllActivated()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAllActivated,
                async () =>
                {
                    var categories = await _unitOfWork.Category.GetAllIsActivated();
                    return _mapper.Map<List<CategoryRes>>(categories);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<CategoryRes?> GetById(long id)
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetById(id),
                async () =>
                {
                    var category = await _unitOfWork.Category.GetById(id);
                    return category == null ? null : _mapper.Map<CategoryRes>(category);
                },
                TimeSpan.FromMinutes(30)
            );
        }

        public async Task<CategoryRes> Create(CategoryReq request)
        {
            var category = _mapper.Map<Category>(request);

            await _unitOfWork.Category.Add(category);
            await _unitOfWork.SaveChanges();

            await ClearCaches();

            return _mapper.Map<CategoryRes>(category);
        }

        public async Task<CategoryRes?> Update(long id, CategoryReq request)
        {
            var category = await _unitOfWork.Category.GetById(id);
            if (category == null)
                return null;

            _mapper.Map(request, category);

            await _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveChanges();

            await ClearCaches();

            return _mapper.Map<CategoryRes>(category);
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
