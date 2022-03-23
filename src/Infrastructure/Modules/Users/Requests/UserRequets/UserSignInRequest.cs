namespace Infrastructure.Modules.Users.Requests.UserRequests
{
    public class UserSignInRequest
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}