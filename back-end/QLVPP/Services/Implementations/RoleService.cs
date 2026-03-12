using AutoMapper;
using QLVPP.DTOs.Response;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class RoleServices : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoleServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<RoleRes>> GetAll()
        {
            var roles = await _unitOfWork.Role.GetAll();
            return _mapper.Map<List<RoleRes>>(roles);
        }
    }
}
