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

        #region Product & Inventory
        public DbSet<Product> Products { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        #endregion

        #region Human Resources
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        #endregion

        #region Stock Operations
        public DbSet<StockIn> StockIns { get; set; }
        public DbSet<StockInDetail> StockInDetails { get; set; }
        public DbSet<StockOut> StockOuts { get; set; }
        public DbSet<StockOutDetail> StockOutDetails { get; set; }
        public DbSet<Requisition> Requisitions { get; set; }
        public DbSet<RequisitionDetail> RequisitionDetails { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<ReturnDetail> ReturnDetails { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TransferDetail> TransferDetails { get; set; }
        #endregion

        #region Inventory Management
        public DbSet<InventorySnapshot> InventorySnapshots { get; set; }
        public DbSet<SnapshotDetail> SnapshotDetails { get; set; }
        public DbSet<StockTake> StockTakes { get; set; }
        public DbSet<StockTakeDetail> StockTakeDetails { get; set; }
        #endregion

        #region Authentication
        public DbSet<InvalidToken> InvalidTokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        #endregion

        #region Approval Flow
        public DbSet<ApprovalConfig> ApprovalConfigs { get; set; }
        public DbSet<ApprovalProcess> ApprovalProcesses { get; set; }
        public DbSet<ApprovalTask> ApprovalTasks { get; set; }
        public DbSet<Approver> Approvers { get; set; }
        #endregion

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

            #region Composite Keys
            modelBuilder.Entity<Inventory>().HasKey(i => new { i.WarehouseId, i.ProductId });
            #endregion

            #region Return Relationships
            modelBuilder.Entity<Return>(entity =>
            {
                entity
                    .HasOne(r => r.Delivery)
                    .WithMany(d => d.Returns)
                    .HasForeignKey(r => r.DeliveryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Transfer Relationships
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
            #endregion

            #region Requisition Relationships
            modelBuilder
                .Entity<Requisition>()
                .HasOne(r => r.Requester)
                .WithMany(e => e.RequisitionsCreated)
                .HasForeignKey(r => r.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region StockOut Relationships
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
            #endregion

            #region StockIn Relationships
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
            #endregion

            #region Employee Relationships
            // Employee → Department
            modelBuilder
                .Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee → Position
            modelBuilder
                .Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Requisition → ApprovalConfig

            modelBuilder
                .Entity<ApprovalConfig>()
                .HasOne(ac => ac.Requisition) 
                .WithOne(r => r.Config)
                .HasForeignKey<ApprovalConfig>(ac => ac.RequisitionId) 
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region ApprovalStep → ApprovalStepApprover

            modelBuilder
                .Entity<Approver>()
                .HasOne(a => a.Config)
                .WithMany(s => s.Approvers)
                .HasForeignKey(a => a.ConfigId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Approver>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.ApproverInSteps)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Requisition → ApprovalInstance

            modelBuilder
                .Entity<ApprovalProcess>()
                .HasOne(i => i.Requisition)
                .WithMany(r => r.Instances)
                .HasForeignKey(i => i.RequisitionId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
            #region ApprovalInstance → ApprovalStepInstance

            modelBuilder
                .Entity<ApprovalTask>()
                .HasOne(s => s.ApprovalInstance)
                .WithMany(i => i.StepInstances)
                .HasForeignKey(s => s.ApprovalInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region ApprovalStepInstance → ApprovalStep

            modelBuilder
                .Entity<ApprovalTask>()
                .HasOne(s => s.Step)
                .WithMany()
                .HasForeignKey(s => s.StepId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region ApprovalStepInstance → Employee (Multiple FKs)

            modelBuilder
                .Entity<ApprovalTask>()
                .HasOne(s => s.AssignedTo)
                .WithMany(e => e.AssignedApprovalSteps)
                .HasForeignKey(s => s.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<ApprovalTask>()
                .HasOne(s => s.ApprovedBy)
                .WithMany(e => e.ProcessedApprovalSteps)
                .HasForeignKey(s => s.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
