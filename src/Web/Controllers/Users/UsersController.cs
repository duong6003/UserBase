using Microsoft.AspNetCore.Mvc;
using Hangfire;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.UserRequests;
using Infrastructure.Modules.Users.Services;
using Microsoft.AspNetCore.Authorization;
using System.Text.Encodings.Web;

namespace Web.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService UserService;
        public UsersController(IUserService userService)
        {
            UserService = userService;
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
            (User? User, string?ErrorMessage) = await UserService.CreateAsync(request);
            if(ErrorMessage is not null) return BadRequest(ErrorMessage);
            return Ok(User, Messages.Users.CreateSuccessfully);
        }
        [HttpPost("signin")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] UserSignInRequest request)
        {
            (string AccesToken, string? errorMessage) = await UserService.AuthenticateAsync(request);
            if(errorMessage is not null) return BadRequest(errorMessage);
            return Ok(AccesToken, Messages.Users.LoginSuccess);
        }
        // [HttpPut("{Id}")]
        // public async Task<IActionResult> Update(UpdateUserRequest request)
        // {
        //     if(errorMessage is not null) return BadRequest(errorMessage);
        //     return Ok(AccesToken, Messages.Users.LoginSuccess);
        // }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync(Guid userId, string token)
        {
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            return Ok();
        }
        private string EmailConfirmationLink(IUrlHelper urlHelper, Guid userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(UsersController.ConfirmEmailAsync),
                controller: "Users",
                values: new { userId, code },
                protocol: scheme)!;
        }
        public string ResetPasswordCallbackLink(IUrlHelper urlHelper, Guid userId, string code, string scheme)
        {
            return urlHelper.Action(
                action: nameof(UsersController.ResetPassword),
                controller: "Users",
                values: new { userId, code },
                protocol: scheme);
        }
        public Task SendEmailConfirmationAsync(IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>click here</a>");
        }
    }
}
