namespace Infrastructure.Modules.Users.Requests.UserPermissionRequests
{
    public class CreateUserPermissionRequest
    {
         public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
    }
}