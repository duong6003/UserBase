using Microsoft.AspNetCore.Mvc;
using Hangfire;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.UserRequests;
using Infrastructure.Modules.Users.Services;
using Microsoft.AspNetCore.Authorization;
using System.Text.Encodings.Web;
using Infrastructure.Persistence.ServiceHelpers.SendMailService;
using Infrastructure.Persistence.ServiceHelpers;
using Infrastructure.Modules.Users.Requests.UserPermissionRequests;

namespace Web.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService UserService;
        private readonly ISendEmail SendEmailService;
        public UsersController(IUserService userService, ISendEmail sendEmailService)
        {
            UserService = userService;
            SendEmailService = sendEmailService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Detail(Guid userId)
        {
            BackgroundJob.Enqueue(() => Console.WriteLine("Simple!"));
            (User? user, string? errorMessage) = await UserService.GetDetailAsync(userId);
            userId = user!.Id;
            if (errorMessage != null)
            {
                return BadRequest(errorMessage);
            }
            return Ok(user, Messages.Users.GetDetailSuccessfully);
        }
        [HttpPost("{userId}/UserPermissions")]
        [AllowAnonymous]
        public async Task<IActionResult> AddUserPermission(Guid userId, [FromBody] List<CreateUserPermissionRequest> request)
        {
            User? user = await UserService.GetByIdAsync(userId);
            if(user == null) return BadRequest(Messages.Users.IdNotFound);
            if (request.Any(x => x.UserId != user.Id)) return Ok(user, Messages.Users.UserIdInCorrect);

            (User? newUser, string? ErrorMessage) = await UserService.AddUserPermissionAsync(user, request);
            if (ErrorMessage is not null) return BadRequest(ErrorMessage);
            return Ok(newUser, Messages.Users.CreateSuccessfully);
        }
        [HttpDelete("{userId}/UserPermissions")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteUserPermission(Guid userId, [FromBody] List<DeleteUserPermissionRequest> request)
        {
            User? user = await UserService.GetByIdAsync(userId);
            if(user == null) return BadRequest(Messages.Users.IdNotFound);
            if (request.Any(x => x.UserId != user.Id)) return Ok(user, Messages.Users.UserIdInCorrect);

            (User? newUser, string? ErrorMessage) = await UserService.DeleteUserPermissionAsync(user, request);
            if (ErrorMessage is not null) return BadRequest(ErrorMessage);
            return Ok(newUser, Messages.Users.CreateSuccessfully);
        }
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromForm] UserSignUpRequest request)
        {
            (User? User, string? ErrorMessage) = await UserService.CreateAsync(request);
            if (ErrorMessage is not null) return BadRequest(ErrorMessage);
            return Ok(User, Messages.Users.CreateSuccessfully);
        }
        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] UserSignInRequest request)
        {
            (string AccesToken, string? errorMessage) = await UserService.AuthenticateAsync(request);
            if (errorMessage is not null) return BadRequest(errorMessage);
            return Ok(AccesToken, Messages.Users.LoginSuccess);
        }
        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(Guid userId, [FromBody] UserUpdateRequest request)
        {
            User? user = await UserService.GetByIdAsync(userId);
            if (user == null) return BadRequest(Messages.Users.IdNotFound);

            (User? newUser, string? errorMessage) = await UserService.UpdateAsync(user, request);
            if (errorMessage is not null) return BadRequest(errorMessage);

            return Ok(newUser, Messages.Users.LoginSuccess);
        }
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            User? user = await UserService.GetByIdAsync(userId);
            if (user == null) return BadRequest(Messages.Users.IdNotFound);

            (User? newUser, string? errorMessage) = await UserService.DeleteAsync(user);

            return Ok(newUser, Messages.Users.LoginSuccess);
        }
        [HttpPost("forgot")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            User? user = await UserService.GetByEmailAsync(request.Email!);
            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            string code = UserService.GeneratePasswordResetTokenAsync(user!);
            var callbackUrl = Url.ResetPasswordCallbackLink(nameof(UsersController.ResetPassword), user!.Id, code, DateTime.UtcNow.AddMinutes(5) , Request.Scheme);

            await SendEmailService.SendEmailAsync(request.Email, "Reset Password", $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return Ok(user, Messages.Users.SendEmailSuccessfully);
        }
        [HttpPost("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordConfirm([FromQuery] Guid userId, [FromQuery] string code, [FromQuery] DateTime expireTime)
        {
            return Ok(code ,await UserService.ConfirmResetPassword(userId, code, expireTime));
        }
        [HttpPost("{userId}/{code}/resetbyconfirm}")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordByConfirm([FromQuery] Guid userId, [FromQuery] string code, ResetPasswordRequest request)
        {
            User? user = await UserService.GetByIdAsync(userId);
            if(user is null) return BadRequest(Messages.Users.IdNotFound);
            (User? updateUser, string? errorMessage) = await UserService.ResetPasswordByConfirm(user!, code, request);
            if (errorMessage != null)
            {
                return BadRequest(errorMessage);
            }
            return Ok(updateUser, Messages.Users.ResetPasswordSuccesfully);
        }
        [HttpPost("reset/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromQuery] Guid userId, [FromBody] ResetPasswordRequest request)
        {
            User? user = await UserService.GetByIdAsync(userId);
            if(user is null) return BadRequest(Messages.Users.IdNotFound);
            (User? updateUser, string? errorMessage) = await UserService.ResetPassword(user!, request);
            if (errorMessage != null)
            {
                return BadRequest(errorMessage);
            }
            return Ok(updateUser, Messages.Users.ResetPasswordSuccesfully);
        }

    }
}
