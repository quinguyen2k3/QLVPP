using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SupplierService(IUnitOfWork unitOfWord, IMapper mapper)
        {
            _unitOfWork = unitOfWord;
            _mapper = mapper;
        }

        public async Task<SupplierRes> Create(SupplierReq request)
        {
            var supplier = _mapper.Map<Supplier>(request);

            await _unitOfWork.Supplier.Add(supplier);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<SupplierRes>(supplier);
        }

        public async Task<List<SupplierRes>> GetAll()
        {
            var suppliers = await _unitOfWork.Supplier.GetAll();
            return _mapper.Map<List<SupplierRes>>(suppliers);
        }

        public async Task<List<SupplierRes>> GetAllActived()
        {
            var suppliers = await _unitOfWork.Supplier.GetAllIsActived();
            return _mapper.Map<List<SupplierRes>>(suppliers);
        }

        public async Task<SupplierRes?> GetById(long id)
        {
            var category = await _unitOfWork.Supplier.GetById(id);
            return category == null ? null : _mapper.Map<SupplierRes>(category);
        }

        public async Task<SupplierRes?> Update(long id, SupplierReq request)
        {
            var supplier = await _unitOfWork.Supplier.GetById(id);
            if (supplier == null)
                return null;

            _mapper.Map(request, supplier);

            await _unitOfWork.Supplier.Update(supplier);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<SupplierRes>(supplier);
        }
    }
}
