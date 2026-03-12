using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLVPP.Models;

namespace QLVPP.Data
{
    public static class PermissionSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var controllers = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                .ToList();

            var dbPermissions = await context.Permissions.ToListAsync();
            var newPermissions = new List<Permission>();

            foreach (var controller in controllers)
            {
                var controllerName = controller.Name.Replace("Controller", "");

                var actions = controller
                    .GetMethods(
                        BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public
                    )
                    .Where(m => !m.IsSpecialName)
                    .Select(m => m.Name)
                    .Distinct()
                    .ToList();

                foreach (var action in actions)
                {
                    var permissionName = $"{controllerName}.{action}";

                    if (!dbPermissions.Any(p => p.Name == permissionName))
                    {
                        newPermissions.Add(
                            new Permission { Name = permissionName, ModuleGroup = controllerName }
                        );
                    }
                }
            }

            if (newPermissions.Any())
            {
                await context.Permissions.AddRangeAsync(newPermissions);
                await context.SaveChangesAsync();

                dbPermissions.AddRange(newPermissions);
            }

            await AssignRolePermissionsAsync(context, dbPermissions);
        }

        private static async Task AssignRolePermissionsAsync(
            AppDbContext context,
            List<Permission> permissions
        )
        {
            var roles = await context.Roles.Include(r => r.RolePermissions).ToListAsync();

            var metadataManager = roles.FirstOrDefault(r => r.Name == "Metadata Manager");
            var warehouseKeeper = roles.FirstOrDefault(r => r.Name == "Warehouse Keeper");
            var warehouseStaff = roles.FirstOrDefault(r => r.Name == "Warehouse Staff");
            var departmentHead = roles.FirstOrDefault(r => r.Name == "Department Head");
            var regularUser = roles.FirstOrDefault(r => r.Name == "Regular User");

            var newRolePermissions = new List<RolePermission>();

            void Assign(Role? role, string[] targetActions)
            {
                if (role == null)
                    return;

                var targetPerms = permissions.Where(p => targetActions.Contains(p.Name)).ToList();

                foreach (var perm in targetPerms)
                {
                    if (!role.RolePermissions.Any(rp => rp.PermissionId == perm.Id))
                    {
                        var newRolePerm = new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = perm.Id,
                        };
                        newRolePermissions.Add(newRolePerm);
                        role.RolePermissions.Add(newRolePerm);
                    }
                }
            }

            Assign(
                metadataManager,
                new[]
                {
                    "Category.Create",
                    "Category.Update",
                    "Category.Delete",
                    "Category.GetById",
                    "Product.Create",
                    "Product.Update",
                    "Product.Delete",
                    "Product.GetById",
                    "Unit.Create",
                    "Unit.Update",
                    "Unit.Delete",
                    "Unit.GetById",
                    "Warehouse.Create",
                    "Warehouse.Update",
                    "Warehouse.Delete",
                    "Warehouse.GetById",
                    "Employee.Create",
                    "Employee.Update",
                    "Employee.Delete",
                    "Employee.GetById",
                    "Department.Create",
                    "Department.Update",
                    "Department.Delete",
                    "Department.GetById",
                    "Supplier.Create",
                    "Supplier.Update",
                    "Supplier.Delete",
                    "Supplier.GetById",
                    "Position.Create",
                    "Position.Update",
                    "Position.Delete",
                    "Position.GetById",
                }
            );

            Assign(
                warehouseKeeper,
                new[]
                {
                    "MaterialRequest.GetPendingByWarehouse",
                    "MaterialRequest.GetApprovedByWarehouse",
                    "MaterialRequest.GetById",
                    "MaterialRequest.Approve",
                    "MaterialRequest.Reject",
                    "StockOut.GetPendingByWarehouse",
                    "StockOut.GetByWarehouse",
                    "StockOut.GetById",
                    "StockOut.Approve",
                    "StockOut.Cancel",
                    "StockOut.GetTransferIncoming",
                    "StockOut.GetAllByMyself",
                    "StockOut.Create",
                    "StockOut.Update",
                    "StockOut.Delete",
                    "StockOut.GetReceivedTransfers",
                    "StockIn.GetPendingByWarehouse",
                    "StockIn.GetByWarehouse",
                    "StockIn.GetById",
                    "StockIn.Approve",
                    "StockIn.Cancel",
                    "StockIn.GetAllByMyself",
                    "StockIn.Create",
                    "StockIn.Update",
                    "StockIn.Delete",
                    "StockTake.GetByWarehouse",
                    "StockTake.GetById",
                    "StockTake.Approve",
                    "StockTake.Cancel",
                    "StockTake.Create",
                    "StockTake.Update",
                    "StockTake.Delete",
                    "Inventory.GetByWarehouse",
                    "Inventory.GetById",
                    "Inventory.Create",
                }
            );

            Assign(
                warehouseStaff,
                new[]
                {
                    "StockIn.GetAllByMyself",
                    "StockIn.GetById",
                    "StockIn.Create",
                    "StockIn.Update",
                    "StockIn.Delete",
                    "StockOut.GetAllByMyself",
                    "StockOut.GetById",
                    "StockOut.Create",
                    "StockOut.Update",
                    "StockOut.Delete",
                    "StockTake.GetAllByWarehouse",
                    "StockTake.GetById",
                    "StockTake.Create",
                    "StockTake.Update",
                    "StockTake.Delete",
                }
            );

            Assign(
                departmentHead,
                new[]
                {
                    "MaterialRequest.GetPendingByDepartment",
                    "MaterialRequest.GetById",
                    "MaterialRequest.Approve",
                    "MaterialRequest.Reject",
                    "MaterialRequest.Delegate",
                    "StockOut.GetApprovedForDepartment",
                    "StockOut.GetReceivedForDepartment",
                    "StockOut.GetById",
                    "StockOut.Receive",
                }
            );

            Assign(
                regularUser,
                new[]
                {
                    "MaterialRequest.GetMyRequests",
                    "MaterialRequest.GetById",
                    "MaterialRequest.Create",
                    "MaterialRequest.Update",
                    "MaterialRequest.Delete",
                    "StockOut.GetById",
                    "StockOut.Receive",
                    "StockOut.GetReceivedForDepartment",
                    "StockOut.GetApprovedForDepartment",
                }
            );

            if (newRolePermissions.Any())
            {
                await context.RolePermissions.AddRangeAsync(newRolePermissions);
                await context.SaveChangesAsync();
            }
        }
    }
}
