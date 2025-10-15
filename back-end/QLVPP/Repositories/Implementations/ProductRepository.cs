using Microsoft.EntityFrameworkCore;
using QLVPP.Constants;
using QLVPP.Data;
using QLVPP.DTOs.Response;
using QLVPP.Models;

namespace QLVPP.Repositories.Implementations
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
            : base(context)
        {
            _context = context;
        }

        public override async Task<Product?> GetById(object id)
        {
            long longId = Convert.ToInt64(id);
            return await _context
                .Products.Include(p => p.Inventory)
                .ThenInclude(p => p.Warehouse)
                .FirstOrDefaultAsync(p => p.Id == longId);
        }

        public async Task<List<Product>> GetAllIsActivated()
        {
            return await _context.Products.Where(p => p.IsActivated == true).ToListAsync();
        }

        public async Task<List<Product>> GetByIds(IEnumerable<long> ids)
        {
            return await _context.Products.Where(p => ids.Contains(p.Id)).ToListAsync();
        }

        public async Task<List<Product>> GetByWarehouseId(long id)
        {
            return await _context
                .Inventories.Where(inventory => inventory.WarehouseId == id)
                .Select(inventory => inventory.Product)
                .Distinct()
                .ToListAsync();
        }

        public Task<List<ProductReportRes>> GetTotalIn(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        )
        {
            var query = _context
                .OrderDetails.Where(d =>
                    d.Order.WarehouseId == warehouseId
                    && d.Order.OrderDate >= startDate
                    && d.Order.OrderDate <= endDate
                    && d.Order.Status == OrderStatus.Complete
                )
                .GroupBy(d => new { d.ProductId, d.Product.Name })
                .Select(g => new ProductReportRes
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(x => x.Quantity),
                });
            return query.AsNoTracking().ToListAsync();
        }

        public Task<List<ProductReportRes>> GetTotalOut(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        )
        {
            var query = _context
                .DeliveryDetails.Where(d =>
                    d.Delivery.WarehouseId == warehouseId
                    && d.Delivery.DeliveryDate >= startDate
                    && d.Delivery.DeliveryDate <= endDate
                    && d.Delivery.Status == DeliveryStatus.Complete
                )
                .GroupBy(d => new { d.ProductId, ProductName = d.Product.Name })
                .Select(g => new ProductReportRes
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Quantity = g.Sum(x => x.Quantity),
                });

            return query.AsNoTracking().ToListAsync();
        }

        public async Task<List<ProductReportRes>> GetTotalReturnAsync(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        )
        {
            var query = _context
                .ReturnDetails.Where(rd =>
                    rd.Return.WarehouseId == warehouseId
                    && rd.Return.ReturnDate >= startDate
                    && rd.Return.ReturnDate <= endDate
                    && rd.Return.Status == ReturnStatus.Complete
                )
                .GroupBy(rd => new { rd.ProductId, rd.Product.Name })
                .Select(g => new ProductReportRes
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(x => x.ReturnedQuantity),
                });

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
