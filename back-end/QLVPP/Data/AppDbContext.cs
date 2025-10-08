using Microsoft.EntityFrameworkCore;
using QLVPP.Models;
using QLVPP.Services;

namespace QLVPP.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<DeliveryDetail> DeliveryDetails { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Requisition> Requisitions { get; set; }
        public DbSet<RequisitionDetail> RequisitionDetails { get; set; }
        public DbSet<InvalidToken> InvalidTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AssetLoan> AssetLoans { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<ReturnDetail> ReturnDetails { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.CreatedBy = _currentUserService.UserAccount ?? "system";
                    entry.Entity.IsActived = true;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedDate = DateTime.Now;
                    entry.Entity.ModifiedBy = _currentUserService.UserAccount ?? "system";
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Inventory>()
                .HasKey(i => new { i.WarehouseId, i.ProductId });

            modelBuilder.Entity<ReturnDetail>(entity =>
            {
                entity.HasOne(detail => detail.Return)
                    .WithMany(r => r.ReturnDetails)
                    .HasForeignKey(detail => detail.ReturnId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(detail => detail.AssetLoan)
                    .WithMany(a => a.ReturnDetails)
                    .HasForeignKey(detail => detail.AssetLoanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AssetLoan>(entity =>
            {
                entity.HasOne(a => a.DeliveryDetail)
                    .WithOne(d => d.AssetLoan)
                    .HasForeignKey<AssetLoan>(a => a.DeliveryDetailId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DeliveryDetail>(entity =>
            {
                entity.HasOne(d => d.Delivery)
                    .WithMany(h => h.DeliveryDetails)
                    .HasForeignKey(d => d.DeliveryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
