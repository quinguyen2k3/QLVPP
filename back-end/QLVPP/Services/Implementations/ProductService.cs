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
        private readonly ICurrentUserService _currentUserService;

        public ProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService
        )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<ProductRes> Create(ProductReq request)
        {
            var product = _mapper.Map<Product>(request);

            await _unitOfWork.Product.Add(product);

            var stock = new Inventory
            {
                Product = product,
                WarehouseId = request.WarehouseId,
                Quantity = 0,
            };

            await _unitOfWork.Inventory.Add(stock);
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

        public async Task<List<ProductRes>> GetByWarehouse()
        {
            var warehouseId = _currentUserService.GetWarehouseId();
            var products = await _unitOfWork.Product.GetByWarehouseId(warehouseId);
            return _mapper.Map<List<ProductRes>>(products);
        }

        public async Task<ProductRes?> Update(long id, ProductReq request)
        {
            var product = await _unitOfWork.Product.GetById(id);
            if (product == null)
                return null;

            if (product.Inventory.WarehouseId != _currentUserService.GetWarehouseId())
            {
                throw new UnauthorizedAccessException(
                    "You are not allowed to update or approve a record from another warehouse."
                );
            }

            _mapper.Map(request, product);

            var oldInventory = await _unitOfWork.Inventory.GetByProductId(id);
            if (oldInventory != null && oldInventory.WarehouseId != request.WarehouseId)
            {
                var oldQuantity = oldInventory.Quantity;

                await _unitOfWork.Inventory.Delete(oldInventory);

                var newInventory = new Inventory
                {
                    ProductId = product.Id,
                    WarehouseId = request.WarehouseId,
                    Quantity = oldQuantity,
                };
                await _unitOfWork.Inventory.Add(newInventory);
            }

            await _unitOfWork.Product.Update(product);
            await _unitOfWork.SaveChanges();

            return _mapper.Map<ProductRes>(product);
        }
    }
}
