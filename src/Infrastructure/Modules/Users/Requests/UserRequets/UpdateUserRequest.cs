using Infrastructure.Modules.Users.Requests.UserPermissionRequests;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Modules.Users.Requests.UserRequests
{
    public class UpdateUserRequest
    {
        public string? UserName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public IFormFile? Avatar { get; set; }
        public Guid? RoleId { get; set; }
        public List<UpdateUserPermission>? UserPermissions { get; set; }
    }
}