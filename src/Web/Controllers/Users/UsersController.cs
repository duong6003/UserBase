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
        public async Task<IActionResult> Update(Guid userId,[FromBody]UserUpdateRequest request)
        {
            User? user = await UserService.GetByIdAsync(userId);
            if (user == null) return BadRequest(Messages.Users.IdNotFound);

            (User? newUser, string? errorMessage) = await UserService.UpdateAsync(user, request);
            if (errorMessage is not null) return BadRequest(errorMessage);

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
            var callbackUrl = Url.ResetPasswordCallbackLink(nameof(UsersController.ResetPassword), user!.Id, code, Request.Scheme);

            await SendEmailService.SendEmailAsync(request.Email, "Reset Password", $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

            return Ok(user, Messages.Users.SendEmailSuccessfully);
        }
        [HttpPost("confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordConfirm([FromQuery]Guid userId, [FromQuery]string code)
        {
            return Ok(await UserService.ConfirmResetPassword(userId, code));
        }
        [HttpPost("reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequest request)
        {
            User? user = await UserService.GetByEmailAsync(request.Email!);
            (User? updateUser, string? errorMessage) = await UserService.ResetPassword(request);
            if (errorMessage != null)
            {
                return BadRequest(errorMessage);
            }
            return Ok(updateUser, Messages.Users.ResetPasswordSuccesfully);
        }

    }
}
