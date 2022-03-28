namespace Infrastructure.Modules.Users.Requests.UserRequests
{
    public class ResetPasswordRequest
    {
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}