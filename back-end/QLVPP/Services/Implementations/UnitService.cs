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

        public UnitService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<UnitRes> Create(UnitReq request)
        {
            var unit = _mapper.Map<Unit>(request);

            await _unitOfWork.Unit.Add(unit);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<UnitRes>(unit);
        }

        public async Task<List<UnitRes>> GetAll()
        {
            var units = await _unitOfWork.Unit.GetAll();
            return _mapper.Map<List<UnitRes>>(units);
        }

        public async Task<List<UnitRes>> GetAllActived()
        {
            var units = await _unitOfWork.Unit.GetAllIsActived();
            return _mapper.Map<List<UnitRes>>(units);
        }

        public async Task<UnitRes?> GetById(long id)
        {
            var unit = await _unitOfWork.Unit.GetById(id);
            return unit == null ? null : _mapper.Map<UnitRes>(unit);
        }

        public async Task<UnitRes?> Update(long id, UnitReq request)
        {
            var unit = await _unitOfWork.Unit.GetById(id);
            if (unit == null)
                return null;

            _mapper.Map(request, unit);

            await _unitOfWork.Unit.Update(unit);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<UnitRes>(unit);
        }
    }
}
