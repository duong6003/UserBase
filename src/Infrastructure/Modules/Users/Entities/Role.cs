using Core.Bases;

namespace Infrastructure.Modules.Users.Entities
{
    public class Role : BaseEntity
    {
        public string? Name { get; set; }
        public virtual ICollection<User>? Users { get; set; }
        public virtual ICollection<RolePermission>? RolePermissions { get; set; }
    }
}