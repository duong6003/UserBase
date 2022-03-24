namespace Infrastructure.Modules.Users.Requests.RolePermissionRequests
{
    public class UpdateRolePermissionRequest
    {
        public Guid? RoleId { get; set; }
        public Guid? PermissionId { get; set; }
    }
}