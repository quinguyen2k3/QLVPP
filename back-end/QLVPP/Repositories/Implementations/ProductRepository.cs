using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.DTOs.Projection;
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

        public override async Task<List<Product>> GetAll()
        {
            return await _context
                .Products.Include(p => p.Inventory)
                .OrderByDescending(p => p.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Product?> GetById(object id)
        {
            long longId = Convert.ToInt64(id);
            return await _context
                .Products.Include(p => p.Inventory)
                .ThenInclude(p => p.Warehouse)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == longId);
        }

        public async Task<List<Product>> GetAllIsActivated()
        {
            return await _context
                .Products.Where(p => p.IsActivated == true)
                .OrderByDescending(p => p.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Product>> GetByIds(IEnumerable<long> ids)
        {
            return await _context
                .Products.Where(p => ids.Contains(p.Id))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Product>> GetByWarehouseId(long id)
        {
            var products = await _context
                .Products.Where(p => p.Inventory.WarehouseId == id)
                .Include(p => p.Inventory)
                .Include(p => p.Unit)
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

            return products;
        }

        public Task<List<ProductReportProj>> GetTotalIn(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        )
        {
            var query = _context
                .StockInDetails.Where(d =>
                    d.StockIn.WarehouseId == warehouseId
                    && d.StockIn.StockInDate >= startDate
                    && d.StockIn.StockInDate <= endDate
                    && d.StockIn.Status == StockInStatus.Approve
                )
                .GroupBy(d => new { d.ProductId, d.Product.Name })
                .Select(g => new ProductReportProj
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(x => x.Quantity),
                });
            return query.AsNoTracking().ToListAsync();
        }

        public Task<List<ProductReportProj>> GetTotalOut(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        )
        {
            var query = _context
                .StockOutDetails.Where(d =>
                    d.Delivery.WarehouseId == warehouseId
                    && d.Delivery.DeliveryDate >= startDate
                    && d.Delivery.DeliveryDate <= endDate
                    && d.Delivery.Status == StockOutStatus.Approved
                )
                .GroupBy(d => new { d.ProductId, ProductName = d.Product.Name })
                .Select(g => new ProductReportProj
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Quantity = g.Sum(x => x.Quantity),
                });

            return query.AsNoTracking().ToListAsync();
        }

        public async Task<List<ProductReportProj>> GetTotalReturnAsync(
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
                    && rd.Return.Status == ReturnStatus.Returned
                )
                .GroupBy(rd => new { rd.ProductId, rd.Product.Name })
                .Select(g => new ProductReportProj
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(x => x.ReturnedQuantity),
                });

            return await query.AsNoTracking().ToListAsync();
        }
    }
}
