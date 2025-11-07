namespace QLVPP.Constants.Permissions
{
    public static class Permissions
    {
        public static class Products
        {
            public const string ViewWarehouse = "products.view.warehouse";
            public const string ViewAll = "products.view.all";
            public const string Create = "products.create";
            public const string Edit = "products.edit";
        }

        public static class Catalog
        {
            public const string ManageCategory = "categories.manage";
            public const string ManageUnit = "units.manage";
            public const string ManageSupplier = "suppliers.manage";
        }

        public static class Employee
        {
            public const string ManageEmployee = "employees.manage";
        }

        public static class Department
        {
            public const string ManageDepartment = "departments.manage";
        }

        public static class Warehouses
        {
            public const string Manage = "warehouses.manage";
        }

        public static class Order
        {
            public const string ManageOwn = "orders.manage.own";
            public const string ViewAll = "orders.view.all";
            public const string Receive = "orders.receive";
            public const string Edit = "orders.edit";
            public const string Delete = "orders.delete";
        }

        public static class Deliveries
        {
            public const string ManageOwn = "deliveries.manage.own";
            public const string Dispatch = "deliveries.dispatch";
            public const string ViewWarehouse = "deliveries.view.warehouse";
            public const string Edit = "deliveries.edit";
            public const string Delete = "deliveries.delete";
        }

        public static class Returns
        {
            public const string ManageOwn = "returns.manage.own";
            public const string Receive = "returns.receive";
            public const string ViewWarehouse = "returns.view.warehouse";
            public const string Edit = "returns.edit";
            public const string Delete = "returns.delete";
        }
    }
}
