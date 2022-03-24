
using Infrastructure.Modules.Users.Requests.RolePermissionRequests;

namespace Infrastructure.Modules.Users.Requests.RoleRequests
{
    public class UpdateRoleRequest
    {
        public string? Name { get; set; }
        public ICollection<UpdateRolePermissionRequest>? RolePermissions { get; set; }
    }
}