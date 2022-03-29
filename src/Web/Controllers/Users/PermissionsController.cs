using Envelop.App.Ultilities;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.PermissionRequests;
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
        /// <summary>
        /// Upload Permission
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePermissionRequest request)
        {
            (Permission? Permission, string? errorMessage) = await PermissionService.CreateAsync(request);
            return Ok(Permission, Messages.Permissions.CreateSuccessfully);
        }

        /// <summary>
        /// Update Permission
        /// </summary>
        [HttpPut("{PermissionId}")]
        public async Task<IActionResult> UpdateAsync(Guid PermissionId, [FromBody] UpdatePermissionRequest request)
        {
            Permission? permission = await PermissionService.GetByIdAsync(PermissionId);
            if (permission == null)
            {
                return BadRequest(Messages.Permissions.CodeNotFound);
            }
            (Permission? newPermission, string? errorMessage) = await PermissionService.UpdateAsync(permission, request);
            return Ok(newPermission, Messages.Permissions.UpdatePermissionSuccessfully);
        }


        /// <summary>
        /// Delete Permission
        /// </summary>
        [HttpDelete("{PermissionId}")]
        public async Task<IActionResult> DeleteAsync(Guid PermissionId)
        {
            Permission? permission = await PermissionService.GetByIdAsync(PermissionId);
            if (permission == null)
            {
                return BadRequest(Messages.Permissions.CodeNotFound);
            }
            (Permission? newPermission, string? errorMessage) = await PermissionService.DeleteAsync(permission);
            return Ok(newPermission, Messages.Permissions.DeletePermissionSuccessfully);
        }
    }
}