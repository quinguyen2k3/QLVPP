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
        private readonly IFileUploadService _fileUploadService;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileUploadService fileUploadService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileUploadService = fileUploadService;
        }

        public async Task<ProductRes> Create(ProductReq request)
        {
            var product = _mapper.Map<Product>(request);

            if (request.Image != null)
            {
                product.ImagePath = await _fileUploadService.UploadAsync(
                    request.Image,
                    UploadFolder.Product
                );
            }

            await _unitOfWork.Product.Add(product);

            await _unitOfWork.SaveChanges();

            return _mapper.Map<ProductRes>(product);
        }

        public async Task<List<ProductRes>> GetAll()
        {
            var products = await _unitOfWork.Product.GetAll();
            return _mapper.Map<List<ProductRes>>(products);
        }

        public async Task<List<ProductRes>> GetAllActivated()
        {
            var products = await _unitOfWork.Product.GetAllIsActivated();
            return _mapper.Map<List<ProductRes>>(products);
        }

        public async Task<ProductRes?> GetById(long id)
        {
            var product = await _unitOfWork.Product.GetById(id);
            return product == null ? null : _mapper.Map<ProductRes>(product);
        }

        public async Task<List<ProductRes>> GetByWarehouse(long id)
        {
            var products = await _unitOfWork.Product.GetByWarehouseId(id);
            return _mapper.Map<List<ProductRes>>(products);
        }

        public async Task<ProductRes?> Update(long id, ProductReq request)
        {
            var product = await _unitOfWork.Product.GetById(id);

            if (product == null)
                return null;

            if (request.Image != null)
            {
                product.ImagePath = await _fileUploadService.UploadAsync(
                    request.Image,
                    UploadFolder.Product
                );
            }

            _mapper.Map(request, product);

            await _unitOfWork.Product.Update(product);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<ProductRes>(product);
        }
    }
}
