using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class PositionService : IPositionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PositionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PositionRes> Create(PositionReq request)
        {
            var positionEntity = _mapper.Map<Position>(request);
            await _unitOfWork.Position.Add(positionEntity);
            await _unitOfWork.SaveChanges();
            return _mapper.Map<PositionRes>(positionEntity);
        }

        public async Task<List<PositionRes>> GetAll()
        {
            var positions = await _unitOfWork.Position.GetAll();
            return _mapper.Map<List<PositionRes>>(positions);
        }

        public async Task<PositionRes> GetById(long id)
        {
            var position = await _unitOfWork.Position.GetById(id);

            if (position == null)
            {
                throw new KeyNotFoundException($"Cannot find position with ID: {id}");
            }

            return _mapper.Map<PositionRes>(position);
        }

        public async Task<PositionRes> Update(long id, PositionReq request)
        {
            var existingPosition = await _unitOfWork.Position.GetById(id);

            if (existingPosition == null)
            {
                throw new KeyNotFoundException($"Cannot find position with ID: {id}");
            }

            _mapper.Map(request, existingPosition);
            await _unitOfWork.Position.Update(existingPosition);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<PositionRes>(existingPosition);
        }
    }
}
