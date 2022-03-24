namespace Infrastructure.Modules.Users.Requests.UserPermissionRequests
{
    public class CreateUserPermission
    {
         public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
    }
}