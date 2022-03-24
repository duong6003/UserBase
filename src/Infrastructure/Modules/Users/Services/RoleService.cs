using AutoMapper;
using Envelop.App.Ultilities;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.RoleRequests;
using Infrastructure.Persistence.Repositories;

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
       ));
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
            Role? role = await GetByIdAsync(roleId);
            if (role is null)
            {
                return (null, Messages.Roles.IdNotFound);
            }
            return (role, null);
        }

        public async Task<(Role? Role, string? ErrorMessage)> UpdateAsync(Role role, UpdateRoleRequest request)
        {
            var newRole = Mapper.Map<UpdateRoleRequest,Role>(request);
            //Get role permisstion Detail
            ICollection<RolePermission>? newRolePermissions = newRole.RolePermissions;

            //new role permissions added
            List<RolePermission>? addedRolePermissions = newRolePermissions!.Where(x => x.Id == default).ToList();

            //get updated role permissions
            List<RolePermission>? updatedRolePermissions = newRolePermissions!.Where(x => x.Id != default).ToList();
            
            //Existed RolePermissions
            var existedRolePermissions = RepositoryWrapper.RolePermissions.FindByCondition(x => x.RoleId == role.Id).ToList();
            
            //Clear db
            newRole.RolePermissions!.Clear();

            foreach (RolePermission? rolePermission in updatedRolePermissions)
            {
                await RepositoryWrapper.RolePermissions.UpdateAsync(rolePermission);
            }

            foreach (RolePermission? rolePermission in addedRolePermissions)
            {
                await RepositoryWrapper.RolePermissions.AddAsync(rolePermission);
            }

            await RepositoryWrapper.RolePermissions.DeleteRangeAsync(existedRolePermissions.Except(updatedRolePermissions));

            await RepositoryWrapper.Roles.UpdateAsync(newRole);
            return(role, null);
        }
    }
}