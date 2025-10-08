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

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

            return _mapper.Map<EmployeeRes>(employee);
        }

        public async Task<List<EmployeeRes>> GetAll()
        {
            var employees = await _unitOfWork.Employee.GetAll();
            return _mapper.Map<List<EmployeeRes>>(employees);
        }

        public async Task<List<EmployeeRes>> GetAllActivated()
        {
            var employees = await _unitOfWork.Employee.GetAllIsActivated();
            return _mapper.Map<List<EmployeeRes>>(employees);
        }

        public async Task<EmployeeRes?> GetById(long id)
        {
            var employee = await _unitOfWork.Employee.GetById(id);
            return employee == null ? null : _mapper.Map<EmployeeRes>(employee);
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

            return _mapper.Map<EmployeeRes>(employee);
        }
    }
}
