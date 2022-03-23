namespace Infrastructure.Modules.Users.Requests
{
    public class ResetPasswordRequest
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}