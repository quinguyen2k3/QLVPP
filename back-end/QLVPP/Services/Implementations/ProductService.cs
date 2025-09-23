using AutoMapper;
using QLVPP.DTOs.Request;
using QLVPP.DTOs.Response;
using QLVPP.Models;
using QLVPP.Repositories;

namespace QLVPP.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProductRes> Create(ProductReq request)
        {
            var product = _mapper.Map<Product>(request);

            await _unitOfWork.Product.Add(product);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<ProductRes>(product);
        }

        public async Task<List<ProductRes>> GetAll()
        {
            var products = await _unitOfWork.Product.GetAll();
            return _mapper.Map<List<ProductRes>>(products);
        }

        public async Task<List<ProductRes>> GetAllActived()
        {
            var products = await _unitOfWork.Product.GetAllIsActived();
            return _mapper.Map<List<ProductRes>>(products);
        }

        public async Task<ProductRes?> GetById(long id)
        {
            var product = await _unitOfWork.Product.GetById(id);
            return product == null ? null : _mapper.Map<ProductRes>(product);
        }

        public async Task<ProductRes?> Update(long id, ProductReq request)
        {
            var product = await _unitOfWork.Product.GetById(id);
            if (product == null)
                return null;

            _mapper.Map(request, product);

            await _unitOfWork.Product.Update(product);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<ProductRes>(product);
        }
    }
}
