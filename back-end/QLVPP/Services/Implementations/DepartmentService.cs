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

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DepartmentRes> Create(DepartmentReq request)
        {
            var department = _mapper.Map<Department>(request);

            await _unitOfWork.Department.Add(department);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<DepartmentRes>(department);
        }

        public async Task<List<DepartmentRes>> GetAll()
        {

            var departments = await _unitOfWork.Department.GetAll();
            return _mapper.Map<List<DepartmentRes>>(departments);
        }

        public async Task<List<DepartmentRes>> GetAllActivated()
        {
            var departments = await _unitOfWork.Department.GetAllIsActivated();
            return _mapper.Map<List<DepartmentRes>>(departments);
        }

        public async Task<DepartmentRes?> GetById(long id)
        {
            var department = await _unitOfWork.Department.GetById(id);
            return department == null ? null : _mapper.Map<DepartmentRes>(department);
        }

        public async Task<DepartmentRes?> Update(long id, DepartmentReq request)
        {
            var department = await _unitOfWork.Department.GetById(id);
            if (department == null)
                return null;

            _mapper.Map(request, department);

            await _unitOfWork.Department.Update(department);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<DepartmentRes>(department);
        }
    }
}
