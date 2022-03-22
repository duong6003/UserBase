using Core.Bases;
using Infrastructure.Modules.Users.Entities;

namespace Infrastructure.Modules.Roles.Entities
{
    public class Role : BaseEntity
    {
        public string? Name { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }
}