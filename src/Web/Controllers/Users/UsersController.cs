using Hangfire;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests;
using Infrastructure.Modules.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
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
        [HttpPost]
        public Task<IActionResult> SignUp(UserSignUpRequest request)
        {
            
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(UserSignInRequest request)
        {
            (string AccesToken, string? errorMessage) = await UserService.Authenticate(request);
            if(!string.IsNullOrEmpty(errorMessage))
            {
                return Ok(null!, errorMessage);
            }
            return Ok(AccesToken, Messages.Users.LoginSuccess);
        }
    }
}
