using AutoMapper;
using QLVPP.DTOs.Response;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class InvalidTokenService : IInvalidTokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InvalidTokenService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<InvalidTokenRes?> GetById(string id)
        {
            var token = await _unitOfWork.InvalidToken.GetById(id);
            return token == null ? null : _mapper.Map<InvalidTokenRes>(token);
        }
    }
}
