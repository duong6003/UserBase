using Core.Bases;
using Infrastructure.Modules.Permissions.Entities;
using Infrastructure.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Modules.UserPermissions.Entities
{
    public class UserPermission : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public Guid PermissionId { get; set; }
        public virtual Permission? Permission { get; set; }
    }
}
