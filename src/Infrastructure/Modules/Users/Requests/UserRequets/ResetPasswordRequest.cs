namespace Infrastructure.Modules.Users.Requests.UserRequests
{
    public class ResetPasswordRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}