namespace Infrastructure.Modules.Users.Requests.UserPermissionRequests
{
    public class UpdateUserPermission
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
    }
}