using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private const string CacheKey_GetAll = "departments:all";
        private const string CacheKey_GetAllActivated = "departments:activated";

        private string CacheKey_GetById(long id) => $"departments:{id}";

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<DepartmentRes> Create(DepartmentReq request)
        {
            var department = _mapper.Map<Department>(request);

            await _unitOfWork.Department.Add(department);
            await _unitOfWork.SaveChanges();

            await ClearCaches();

            return _mapper.Map<DepartmentRes>(department);
        }

        public async Task<List<DepartmentRes>> GetAll()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAll,
                async () =>
                {
                    var departments = await _unitOfWork.Department.GetAll();
                    return _mapper.Map<List<DepartmentRes>>(departments);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<List<DepartmentRes>> GetAllActivated()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAllActivated,
                async () =>
                {
                    var departments = await _unitOfWork.Department.GetAllIsActivated();
                    return _mapper.Map<List<DepartmentRes>>(departments);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<DepartmentRes?> GetById(long id)
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetById(id),
                async () =>
                {
                    var department = await _unitOfWork.Department.GetById(id);
                    return department == null ? null : _mapper.Map<DepartmentRes>(department);
                },
                TimeSpan.FromMinutes(30)
            );
        }

        public async Task<DepartmentRes?> Update(long id, DepartmentReq request)
        {
            var department = await _unitOfWork.Department.GetById(id);
            if (department == null)
                return null;

            _mapper.Map(request, department);

            await _unitOfWork.Department.Update(department);
            await _unitOfWork.SaveChanges();

            await ClearCaches(id);

            return _mapper.Map<DepartmentRes>(department);
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
