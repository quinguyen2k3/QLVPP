using Microsoft.EntityFrameworkCore;
using QLVPP.Models;
using QLVPP.Services;

namespace QLVPP.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserService? _currentUserService;

        public AppDbContext(
            DbContextOptions<AppDbContext> options,
            ICurrentUserService? currentUserService = null
        )
            : base(options)
        {
            _currentUserService = currentUserService;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<StockIn> StockIns { get; set; }
        public DbSet<StockInDetail> StockInDetails { get; set; }
        public DbSet<StockOut> StockOuts { get; set; }
        public DbSet<StockOutDetail> StockOutDetails { get; set; }
        public DbSet<InventorySnapshot> InventorySnapshots { get; set; }
        public DbSet<SnapshotDetail> SnapshotDetails { get; set; }
        public DbSet<StockTake> StockTakes { get; set; }
        public DbSet<StockTakeDetail> StockTakeDetails { get; set; }
        public DbSet<InvalidToken> InvalidTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<MaterialRequest> MaterialRequests { get; set; }
        public DbSet<MaterialRequestDetail> MaterialRequestDetails { get; set; }
        public DbSet<ApprovalLog> ApprovalLogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<AuditableEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = _currentUserService?.GetUserAccount() ?? "System";
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.IsActivated = true;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedBy = _currentUserService?.GetUserAccount() ?? "System";
                    entry.Entity.ModifiedDate = DateTime.Now;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MaterialRequest>(entity =>
            {
                entity
                    .HasOne(m => m.Warehouse)
                    .WithMany()
                    .HasForeignKey(m => m.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(m => m.Requester)
                    .WithMany()
                    .HasForeignKey(m => m.RequesterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(m => m.Approver)
                    .WithMany()
                    .HasForeignKey(m => m.ApproverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasMany(m => m.Details)
                    .WithOne(m => m.MaterialRequest)
                    .HasForeignKey(m => m.MaterialRequestId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasMany(m => m.ApprovalLogs)
                    .WithOne(a => a.MaterialRequest)
                    .HasForeignKey(a => a.MaterialRequestId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockIn>(entity =>
            {
                entity
                    .HasOne(s => s.Warehouse)
                    .WithMany()
                    .HasForeignKey(s => s.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(s => s.FromWarehouse)
                    .WithMany()
                    .HasForeignKey(s => s.FromWarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(s => s.Supplier)
                    .WithMany()
                    .HasForeignKey(s => s.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StockOut>(entity =>
            {
                entity
                    .HasOne(d => d.Warehouse)
                    .WithMany()
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(d => d.ToWarehouse)
                    .WithMany()
                    .HasForeignKey(d => d.ToWarehouseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(d => d.Department)
                    .WithMany()
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(d => d.Requester)
                    .WithMany(e => e.DeliveriesRequested)
                    .HasForeignKey(d => d.RequesterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(d => d.Approver)
                    .WithMany(e => e.DeliveriesApproved)
                    .HasForeignKey(d => d.ApproverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(d => d.Receiver)
                    .WithMany(e => e.DeliveriesReceived)
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(d => d.Type).HasConversion<int>();
            });

            modelBuilder
                .Entity<StockTake>()
                .HasOne(s => s.Requester)
                .WithMany(e => e.StockTakesRequested)
                .HasForeignKey(s => s.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<StockTake>()
                .HasOne(s => s.Approver)
                .WithMany(e => e.StockTakesApproved)
                .HasForeignKey(s => s.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventory>().HasKey(i => new { i.WarehouseId, i.ProductId });

            modelBuilder
                .Entity<Inventory>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Inventories)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Inventory>()
                .HasOne(i => i.Warehouse)
                .WithMany(w => w.Inventories)
                .HasForeignKey(i => i.WarehouseId)
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

            modelBuilder
                .Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<RolePermission>()
                .HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                .IsUnique();

            modelBuilder
                .Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder
                .Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);
        }
    }
}
