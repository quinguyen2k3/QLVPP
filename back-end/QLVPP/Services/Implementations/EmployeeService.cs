using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;
using QLVPP.Security;

namespace QLVPP.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private const string CacheKey_GetAll = "employees:all";
        private const string CacheKey_GetAllActivated = "employees:activated";

        private string CacheKey_GetById(long id) => $"employees:{id}";

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<EmployeeRes> Create(EmployeeReq request)
        {
            var existed = await _unitOfWork.Employee.GetByAccount(request.Account);
            if (existed != null)
                throw new Exception("Account already exists.");

            var employee = _mapper.Map<Employee>(request);

            employee.Password = PasswordHasher.HashPassword(request.Password);

            await _unitOfWork.Employee.Add(employee);
            await _unitOfWork.SaveChanges();

            await ClearCaches();

            return _mapper.Map<EmployeeRes>(employee);
        }

        public async Task<List<EmployeeRes>> GetAll()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAll,
                async () =>
                {
                    var employees = await _unitOfWork.Employee.GetAll();
                    return _mapper.Map<List<EmployeeRes>>(employees);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<List<EmployeeRes>> GetAllActivated()
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetAllActivated,
                async () =>
                {
                    var employees = await _unitOfWork.Employee.GetAllIsActivated();
                    return _mapper.Map<List<EmployeeRes>>(employees);
                },
                TimeSpan.FromHours(1)
            );
        }

        public async Task<EmployeeRes?> GetById(long id)
        {
            return await _cacheService.GetOrSet(
                CacheKey_GetById(id),
                async () =>
                {
                    var employee = await _unitOfWork.Employee.GetById(id);
                    return employee == null ? null : _mapper.Map<EmployeeRes>(employee);
                },
                TimeSpan.FromMinutes(30)
            );
        }

        public async Task<EmployeeRes?> Update(long id, EmployeeReq request)
        {
            var employee = await _unitOfWork.Employee.GetById(id);
            if (employee == null)
                return null;

            var existed = await _unitOfWork.Employee.GetByAccount(request.Account);
            if (existed != null && existed.Id != id)
                throw new Exception("Account already exists.");

            _mapper.Map(request, employee);

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                employee.Password = PasswordHasher.HashPassword(request.Password);
            }

            await _unitOfWork.Employee.Update(employee);
            await _unitOfWork.SaveChanges();

            await ClearCaches(id);

            return _mapper.Map<EmployeeRes>(employee);
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
