namespace Infrastructure.Modules.Users.Requests.UserPermissionRequests
{
    public class UpdateUserPermissionRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
    }
}