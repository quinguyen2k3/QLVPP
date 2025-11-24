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
        public DbSet<ApprovalTemplate> ApprovalTemplates { get; set; }
        public DbSet<ApprovalStep> ApprovalTemplateSteps { get; set; }
        public DbSet<ApprovalInstance> ApprovalInstances { get; set; }
        public DbSet<ApprovalStepInstance> ApprovalStepInstances { get; set; }
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

            #region Approval Flow Relationships

            // ✅ ApprovalTemplate → ApprovalTemplateStep
            modelBuilder
                .Entity<ApprovalStep>()
                .HasOne(s => s.Template)
                .WithMany(t => t.Steps)
                .HasForeignKey(s => s.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ ApprovalTemplateStep → Position
            modelBuilder
                .Entity<ApprovalStep>()
                .HasOne(s => s.Position)
                .WithMany(p => p.TemplateSteps)
                .HasForeignKey(s => s.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ ApprovalInstance → ApprovalTemplate
            modelBuilder
                .Entity<ApprovalInstance>()
                .HasOne(i => i.Template)
                .WithMany(t => t.Instances)
                .HasForeignKey(i => i.TemplateId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa Template có Instance

            // ✅ ApprovalInstance → Employee (Requester)
            modelBuilder
                .Entity<ApprovalInstance>()
                .HasOne(i => i.Requester)
                .WithMany(e => e.RequestedApprovals)
                .HasForeignKey(i => i.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ ApprovalInstance → Department
            modelBuilder
                .Entity<ApprovalInstance>()
                .HasOne(i => i.RequesterDepartment)
                .WithMany(d => d.ApprovalInstances)
                .HasForeignKey(i => i.RequesterDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ ApprovalStepInstance → ApprovalInstance
            modelBuilder
                .Entity<ApprovalStepInstance>()
                .HasOne(s => s.ApprovalInstance)
                .WithMany(i => i.StepInstances)
                .HasForeignKey(s => s.ApprovalInstanceId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Instance thì xóa Steps

            // ✅ ApprovalStepInstance → ApprovalTemplateStep
            modelBuilder
                .Entity<ApprovalStepInstance>()
                .HasOne(s => s.TemplateStep)
                .WithMany()
                .HasForeignKey(s => s.TemplateStepId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ ApprovalStepInstance → Position
            modelBuilder
                .Entity<ApprovalStepInstance>()
                .HasOne(s => s.Position)
                .WithMany(p => p.StepInstances)
                .HasForeignKey(s => s.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ ApprovalStepInstance → Employee (AssignedTo)
            modelBuilder
                .Entity<ApprovalStepInstance>()
                .HasOne(s => s.AssignedTo)
                .WithMany(e => e.AssignedSteps)
                .HasForeignKey(s => s.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ ApprovalStepInstance → Employee (ApprovedBy)
            modelBuilder
                .Entity<ApprovalStepInstance>()
                .HasOne(s => s.ApprovedBy)
                .WithMany(e => e.ApprovedSteps)
                .HasForeignKey(s => s.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ ApprovalStepInstance → Department
            modelBuilder
                .Entity<ApprovalStepInstance>()
                .HasOne(s => s.AssignedDepartment)
                .WithMany(d => d.ApprovalStepInstances)
                .HasForeignKey(s => s.AssignedDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Indexes for Approval Flow

            // ApprovalTemplate Indexes
            modelBuilder
                .Entity<ApprovalTemplate>()
                .HasIndex(t => t.NoteType)
                .HasDatabaseName("IX_ApprovalTemplate_NoteType");

            modelBuilder
                .Entity<ApprovalTemplate>()
                .HasIndex(t => t.Code)
                .IsUnique()
                .HasDatabaseName("IX_ApprovalTemplate_Code")
                .HasFilter("[Code] IS NOT NULL");

            modelBuilder
                .Entity<ApprovalTemplate>()
                .HasIndex(t => new { t.NoteType, t.IsDefault })
                .HasDatabaseName("IX_ApprovalTemplate_NoteType_IsDefault");

            // ApprovalTemplateStep Indexes
            modelBuilder
                .Entity<ApprovalStep>()
                .HasIndex(s => s.TemplateId)
                .HasDatabaseName("IX_ApprovalTemplateStep_TemplateId");

            modelBuilder
                .Entity<ApprovalStep>()
                .HasIndex(s => new { s.TemplateId, s.StepOrder })
                .IsUnique()
                .HasDatabaseName("UX_ApprovalTemplateStep_Template_Order");

            // ApprovalInstance Indexes
            modelBuilder
                .Entity<ApprovalInstance>()
                .HasIndex(i => new { i.NoteType, i.NoteId })
                .IsUnique()
                .HasDatabaseName("UX_ApprovalInstance_Note");

            modelBuilder
                .Entity<ApprovalInstance>()
                .HasIndex(i => i.Status)
                .HasDatabaseName("IX_ApprovalInstance_Status");

            modelBuilder
                .Entity<ApprovalInstance>()
                .HasIndex(i => i.RequesterId)
                .HasDatabaseName("IX_ApprovalInstance_RequesterId");

            // ApprovalStepInstance Indexes
            modelBuilder
                .Entity<ApprovalStepInstance>()
                .HasIndex(s => s.ApprovalInstanceId)
                .HasDatabaseName("IX_ApprovalStepInstance_ApprovalInstanceId");

            modelBuilder
                .Entity<ApprovalStepInstance>()
                .HasIndex(s => new { s.ApprovalInstanceId, s.StepOrder })
                .IsUnique()
                .HasDatabaseName("UX_ApprovalStepInstance_Instance_Order");

            modelBuilder
                .Entity<ApprovalStepInstance>()
                .HasIndex(s => s.Status)
                .HasDatabaseName("IX_ApprovalStepInstance_Status");

            modelBuilder
                .Entity<ApprovalStepInstance>()
                .HasIndex(s => s.AssignedToId)
                .HasDatabaseName("IX_ApprovalStepInstance_AssignedToId");

            modelBuilder
                .Entity<ApprovalStepInstance>()
                .HasIndex(s => new { s.AssignedToId, s.Status })
                .HasDatabaseName("IX_ApprovalStepInstance_AssignedTo_Status")
                .HasFilter("[AssignedToId] IS NOT NULL");

            #endregion
        }
    }
}
