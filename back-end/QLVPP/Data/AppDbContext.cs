using Microsoft.EntityFrameworkCore;
using QLVPP.Models;
using QLVPP.Services;

namespace QLVPP.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService _currentUserService;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUserService
        )
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<StockIn> StockIns { get; set; }
        public DbSet<StockInDetail> OrderDetails { get; set; }
        public DbSet<StockOut> Deliveries { get; set; }
        public DbSet<StockOutDetail> DeliveryDetails { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Requisition> Requisitions { get; set; }
        public DbSet<RequisitionDetail> RequisitionDetails { get; set; }
        public DbSet<InvalidToken> InvalidTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<ReturnDetail> ReturnDetails { get; set; }
        public DbSet<InventorySnapshot> InventorySnapshots { get; set; }
        public DbSet<SnapshotDetail> SnapshotDetails { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferDetail> TransferDetails { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<AuditableEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = _currentUserService.GetUserAccount();
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.IsActivated = true;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedBy = _currentUserService.GetUserAccount();
                    entry.Entity.ModifiedDate = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Inventory>().HasKey(i => new { i.WarehouseId, i.ProductId });
            modelBuilder.Entity<Return>(entity =>
            {
                entity
                    .HasOne(r => r.Delivery)
                    .WithMany(d => d.Returns)
                    .HasForeignKey(r => r.DeliveryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder
                .Entity<Transfer>()
                .HasOne(t => t.FromWarehouse)
                .WithMany(w => w.TransfersFrom)
                .HasForeignKey(t => t.FromWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Transfer>()
                .HasOne(t => t.ToWarehouse)
                .WithMany(w => w.TransfersTo)
                .HasForeignKey(t => t.ToWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Requisition>()
                .HasOne(r => r.Requester)
                .WithMany(e => e.RequisitionsCreated)
                .HasForeignKey(r => r.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Requisition>()
                .HasOne(r => r.OriginalApprover)
                .WithMany(e => e.RequisitionsToOriginallyApprove)
                .HasForeignKey(r => r.OriginalApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<StockOut>()
                .HasOne(d => d.Requester)
                .WithMany(e => e.DeliveriesRequested)
                .HasForeignKey(d => d.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<StockOut>()
                .HasOne(d => d.Approver)
                .WithMany(e => e.DeliveriesApproved)
                .HasForeignKey(d => d.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<StockOut>()
                .HasOne(d => d.Receiver)
                .WithMany(e => e.DeliveriesReceived)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Transfer>()
                .HasOne(t => t.Requester)
                .WithMany(e => e.TransfersRequested)
                .HasForeignKey(t => t.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Transfer>()
                .HasOne(t => t.Approver)
                .WithMany(e => e.TransfersApproved)
                .HasForeignKey(t => t.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Transfer>()
                .HasOne(t => t.Receiver)
                .WithMany(e => e.TransfersReceived)
                .HasForeignKey(t => t.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<StockIn>()
                .HasOne(s => s.Requester)
                .WithMany(e => e.StockInsRequested)
                .HasForeignKey(s => s.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<StockIn>()
                .HasOne(s => s.Approver)
                .WithMany(e => e.StockInsApproved)
                .HasForeignKey(s => s.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
