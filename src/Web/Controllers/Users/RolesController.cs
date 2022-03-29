using Envelop.App.Ultilities;
using Hangfire;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.RoleRequests;
using Infrastructure.Modules.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : BaseController
    {
        private readonly IRoleService RoleService;

        public RolesController(IRoleService roleService)
        {
            RoleService = roleService;
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> Detail(Guid roleId)
        {
            BackgroundJob.Enqueue(() => Console.WriteLine("Simple!"));
            (Role? role, string? errorMessage) = await RoleService.GetDetailAsync(roleId);
            roleId = role!.Id;
            if (errorMessage != null)
            {
                return BadRequest(errorMessage);
            }
            return Ok(role, Messages.Roles.GetDetailSuccessfully);
        }

        /// <summary>
        /// Upload Role
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateRoleRequest request)
        {
            (Role? role, string? errorMessage) = await RoleService.CreateAsync(request);
            return Ok(role, Messages.Roles.CreateSuccessfully);
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest request)
        {
            (PaginationResponse<Role> roles, string? errorMessage) = await RoleService.GetAllAsync(request);
            return Ok(roles, Messages.Roles.GetAllSuccessfully);
        }

        /// <summary>
        /// Update Role
        /// </summary>
        [HttpPut("{roleId}")]
        public async Task<IActionResult> UpdateAsync(Guid roleId, [FromBody] UpdateRoleRequest request)
        {
            Role? role = await RoleService.GetByIdAsync(roleId);
            if (role == null)
            {
                return BadRequest(Messages.Roles.IdNotFound);
            }
            (Role? Role, string? errorMessage) = await RoleService.UpdateAsync(role, request);
            return Ok(Role, Messages.Roles.UpdateRoleSuccessfully);
        }

        /// <summary>
        /// Delete Role
        /// </summary>
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteAsync(Guid roleId)
        {
            Role? role = await RoleService.GetByIdAsync(roleId);
            if (role == null)
            {
                return BadRequest(Messages.Roles.IdNotFound);
            }
            (Role? Role, string? errorMessage) = await RoleService.DeleteAsync(role);
            return Ok(Role, Messages.Roles.DeleteRoleSuccessfully);
        }
    }
}