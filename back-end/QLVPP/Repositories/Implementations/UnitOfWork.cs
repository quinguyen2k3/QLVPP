﻿using QLVPP.Data;
using QLVPP.Models;
using QLVPP.Security;

namespace QLVPP.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public IUnitRepository Unit { get; private set; }
        public IEmployeeRepository Employee { get; private set; }
        public IRefreshTokenRepository RefreshToken { get; private set; }
        public IInvalidTokenRepository InvalidToken { get; private set; }
        public IDepartmentRepository Department {  get; private set; }
        public ISupplierRepository Supplier {  get; private set; }
        public IWarehouseRepository Warehouse { get; private set; }
        public IRequisitionRepository Requisition { get; private set; }
        public IProductRepository Product { get; private set; }
        public IOrderRepository Order { get; private set; }
        public IInventoryRepository Inventory { get; private set; }
        public IDeliveryRepository Delivery { get; private set; }
        public IAssetLoanRepository AssetLoan { get; private set; }
        public IReturnRepository Return { get; private set; }

        public readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            Category = new CategoryRepository(context);
            Unit = new UnitRepository(context);
            Employee = new EmployeeRepository(context);
            RefreshToken = new RefreshTokenRepository(context);
            InvalidToken = new InvalidTokenRepository(context);
            Department = new DepartmentRepository(context);
            Supplier = new SupplierRepository(context);
            Warehouse = new WarehouseRepository(context);
            Requisition = new RequisitionRepository(context);
            Product = new ProductRepository(context);
            Order = new OrderRepository(context);
            Inventory = new InventoryRepository(context);
            Delivery = new DeliveryRepository(context);
            AssetLoan = new AssetLoanRepository(context);
            Return = new ReturnRepository(context);
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

    }
}
