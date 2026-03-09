namespace QLVPP.Models
{
    public class Permission : AuditableEntity
    {
        public required string Name { get; set; }
        public string? ModuleGroup { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } =
            new List<RolePermission>();
    }
}
