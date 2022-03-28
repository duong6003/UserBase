namespace Infrastructure.Modules.Users.Requests.UserPermissionRequests
{
    public class DeleteUserPermissionRequest
    {
        public Guid UserId { get; set; }
        public string? Code { get; set; }
    }
}