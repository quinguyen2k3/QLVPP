namespace QLVPP.Models
{
    public class Role : AuditableEntity
    {
        public required string Name { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<RolePermission> RolePermissions { get; set; } =
            new List<RolePermission>();
    }
}
