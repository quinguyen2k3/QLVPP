using System.ComponentModel.DataAnnotations.Schema;

namespace QLVPP.Models
{
    public class RolePermission : BaseEntity
    {
        public long RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; } = null!;

        public long PermissionId { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public Permission Permission { get; set; } = null!;
    }
}
