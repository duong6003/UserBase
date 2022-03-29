using Microsoft.AspNetCore.Http;

namespace Infrastructure.Modules.Users.Requests.UserRequests
{
    public class UserSignUpRequest
    {
        public string? UserName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public IFormFile? Avatar { get; set; }
        public Guid? RoleId { get; set; }
    }
}