namespace Infrastructure.Modules.Users.Requests.UserPermissionRequests
{
    public class UpdateUserPermissionRequest
    {
        public Guid UserId { get; set; }
        public string? Code { get; set; }
    }
}