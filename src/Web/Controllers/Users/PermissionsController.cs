using Envelop.App.Ultilities;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionsController : BaseController
    {
        private readonly IPermissionService PermissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            PermissionService = permissionService;
        }
           /// <summary>
        /// Get All Permission
        /// </summary>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest request)
        {
            (PaginationResponse<Permission> permissions, string? errorMessage) = await PermissionService.GetAllAsync(request);
            return Ok(permissions, Messages.Permissions.GetAllSuccessfully);
        }
    }
}