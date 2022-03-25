using AutoMapper;
using Envelop.App.Ultilities;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.RolePermissionRequests;
using Infrastructure.Modules.Users.Requests.RoleRequests;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Modules.Users.Services
{
    public interface IRoleService
    {
        Task<Role?> GetByIdAsync(Guid roleId);
        Task<(Role? Role, string? ErrorMessage)> GetDetailAsync(Guid roleId);
        Task<(PaginationResponse<Role>, string? ErrorMessage)> GetAllAsync(PaginationRequest request);
        Task<(Role? Role, string? ErrorMessage)> CreateAsync(CreateRoleRequest request);
        Task<(Role? Role, string? ErrorMessage)> UpdateAsync(Role role, UpdateRoleRequest request);
        Task<(Role? Role, string? ErrorMessage)> DeleteAsync(Role role);
    }
    public class RoleService : IRoleService
    {
        private readonly IRepositoryWrapper RepositoryWrapper;
        private readonly IMapper Mapper;

        public RoleService(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            RepositoryWrapper = repositoryWrapper;
            Mapper = mapper;
        }
        public async Task<(Role? Role, string? ErrorMessage)> CreateAsync(CreateRoleRequest request)
        {
            Role? role = Mapper.Map<Role>(request);
            await RepositoryWrapper.Roles.AddAsync(role);
            return (role, null);
        }

        public async Task<(Role? Role, string? ErrorMessage)> DeleteAsync(Role role)
        {
            await RepositoryWrapper.Roles.DeleteAsync(role);
            return (role, Messages.Roles.DeleteRoleSuccessfully);
        }

        public async Task<(PaginationResponse<Role>, string? ErrorMessage)> GetAllAsync(PaginationRequest request)
        {
            IQueryable<Role>? roles = RepositoryWrapper.Roles.FindByCondition(x =>
                (
                    string.IsNullOrEmpty(request.SearchContent)
                    || x.Name!.ToLower().Contains(request.SearchContent!.ToLower())
                )).Include(x=> x.RolePermissions);
            roles = SortUtility<Role>.ApplySort(roles, request.OrderByQuery!);
            PaginationUtility<Role>? data = await PaginationUtility<Role>.ToPagedListAsync(roles, request.PageNumber, request.PageSize);
            return (PaginationResponse<Role>.PaginationInfo(data, data.PageInfo), Messages.Roles.GetAllSuccessfully);
        }

        public async Task<Role?> GetByIdAsync(Guid roleId)
        {
            return await RepositoryWrapper.Roles.GetByIdAsync(roleId);
        }

        public async Task<(Role? Role, string? ErrorMessage)> GetDetailAsync(Guid roleId)
        {
            Role? role = await RepositoryWrapper.Roles.FindByCondition(x=> x.Id == roleId).Include(x=> x.RolePermissions).FirstOrDefaultAsync();
            if (role is null)
            {
                return (null, Messages.Roles.IdNotFound);
            }
            return (role, null);
        }

        public async Task<(Role? Role, string? ErrorMessage)> UpdateAsync(Role role, UpdateRoleRequest request)
        {
            //Get role permisstion Detail
            var newRolePermissions = request.RolePermissions;

            //new role permissions added
            var addedRolePermissionsReq = newRolePermissions!.Where(x => x.Id == Guid.Empty).ToList();
            var addedRolePermissions = Mapper.Map<List<RolePermission>>(addedRolePermissionsReq);

            //get updated role permissions
            var updatedRolePermissionsReq = newRolePermissions!.Where(x => x.Id != Guid.Empty).ToList();
            var updatedRolePermissions = Mapper.Map<List<RolePermission>>(updatedRolePermissionsReq);

            //Existed RolePermissions
            var existedRolePermissions = RepositoryWrapper.RolePermissions.FindByCondition(x => x.RoleId == role.Id).ToList();

            foreach (var rolePermission in addedRolePermissions)
            {
                await RepositoryWrapper.RolePermissions.AddAsync(rolePermission);
            }

            foreach (var rolePermission in updatedRolePermissions)
            {
                await RepositoryWrapper.RolePermissions.UpdateAsync(rolePermission);
            }

            await RepositoryWrapper.RolePermissions.DeleteRangeAsync(existedRolePermissions.Except(updatedRolePermissions));

            role.Name = request.Name;

            await RepositoryWrapper.Roles.UpdateAsync(role);
            return(role, null);
        }
    }
}