using Infrastructure.Modules.Users.Requests.RolePermissionRequests;

namespace Infrastructure.Modules.Users.Requests.RoleRequests
{
    public class CreateRoleRequest
    {
        public string? Name { get; set; }
        public ICollection<CreateRolePermissionRequest>? RolePermissions { get; set; }
    }
}