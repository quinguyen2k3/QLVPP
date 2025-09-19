using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WarehouseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WarehouseRes> Create(WarehouseReq request)
        {
            var warehouse = _mapper.Map<Warehouse>(request);

            await _unitOfWork.Warehouse.Add(warehouse);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<WarehouseRes>(warehouse);
        }

        public async Task<List<WarehouseRes>> GetAll()
        {
            var warehouse = await _unitOfWork.Warehouse.GetAll();
            return _mapper.Map<List<WarehouseRes>>(warehouse);
        }

        public async Task<List<WarehouseRes>> GetAllActived()
        {
            var warehouse = await _unitOfWork.Warehouse.GetAllIsActived();
            return _mapper.Map<List<WarehouseRes>>(warehouse);
        }

        public async Task<WarehouseRes?> GetById(long id)
        {
            var warehouse = await _unitOfWork.Warehouse.GetById(id);
            return warehouse == null ? null : _mapper.Map<WarehouseRes>(warehouse);
        }

        public async Task<WarehouseRes?> Update(long id, WarehouseReq request)
        {
            var warehouse = await _unitOfWork.Warehouse.GetById(id);
            if (warehouse == null)
                return null;

            _mapper.Map(request, warehouse);

            await _unitOfWork.Warehouse.Update(warehouse);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<WarehouseRes>(warehouse);
        }
    }
}
