namespace Infrastructure.Modules.Users.Requests.RolePermissionRequests
{
    public class CreateRolePermissionRequest
    {
        public Guid? RoleId { get; set; }
        public Guid? PermissionId { get; set; }
    }
}