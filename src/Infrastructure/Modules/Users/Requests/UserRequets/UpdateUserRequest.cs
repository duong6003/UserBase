using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Modules.Users.Requests.UserRequests
{
    public class UpdateUserRequest
    {
        public string? UserName { get; set; }
        public string? EmailAddress  { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public IFormFile? Avatar { get; set; }
        public Guid? RoleId { get; set; }
    }
}