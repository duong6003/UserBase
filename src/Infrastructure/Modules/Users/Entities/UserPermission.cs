using Core.Bases;

namespace Infrastructure.Modules.Users.Entities
{
    public class UserPermission : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public Guid PermissionId { get; set; }
        public virtual Permission? Permission { get; set; }
    }
}
