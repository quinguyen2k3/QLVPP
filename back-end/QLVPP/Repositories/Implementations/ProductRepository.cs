using Microsoft.EntityFrameworkCore;
using QLVPP.Constants.Status;
using QLVPP.Data;
using QLVPP.DTOs.Result;
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
                .Products.OrderByDescending(p => p.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Product?> GetById(object id)
        {
            long longId = Convert.ToInt64(id);
            return await _context
                .Products.Include(p => p.Inventories)
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
                .Products.Include(p => p.Unit)
                .Include(p => p.Category)
                .Include(p => p.Inventories.Where(i => i.WarehouseId == id))
                .Where(p => p.Inventories.Any(i => i.WarehouseId == id))
                .AsNoTracking()
                .ToListAsync();

            return products;
        }

        public Task<List<ProductReportResult>> GetTotalIn(
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
                .Select(g => new ProductReportResult
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Quantity = g.Sum(x => x.Quantity),
                });
            return query.AsNoTracking().ToListAsync();
        }

        public Task<List<ProductReportResult>> GetTotalOut(
            long warehouseId,
            DateOnly startDate,
            DateOnly endDate
        )
        {
            var query = _context
                .StockOutDetails.Where(d =>
                    d.StockOut.WarehouseId == warehouseId
                    && d.StockOut.StockOutDate >= startDate
                    && d.StockOut.StockOutDate <= endDate
                    && d.StockOut.Status == StockOutStatus.Approved
                )
                .GroupBy(d => new { d.ProductId, ProductName = d.Product.Name })
                .Select(g => new ProductReportResult
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    Quantity = g.Sum(x => x.Quantity),
                });

            return query.AsNoTracking().ToListAsync();
        }
    }
}
