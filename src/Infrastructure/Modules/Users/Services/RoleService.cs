using AutoMapper;
using Envelop.App.Ultilities;
using Hangfire;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.RoleRequests;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Modules.Users.Services
{
    //public class RolePermissionCompare : IEqualityComparer<RolePermission>
    //{
    //    public bool Equals(RolePermission? x, RolePermission? y)
    //    {
    //        return x!.RoleId == y!.RoleId && x.Code == y.Code;
    //    }

    //    public int GetHashCode([DisallowNull] RolePermission obj)
    //    {
    //        unchecked
    //        {
    //            if (obj == null)
    //                return 0;
    //            return obj.GetHashCode();
    //        }
    //    }
    //}

    public interface IRoleService
    {
        Task<Role?> GetByIdAsync(Guid roleId);
        Task<Role?> GetAllRolePermission(Role role);

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
            Role? newRole = await GetAllRolePermission(role);
            return (newRole , null);
        }

        public async Task<(Role? Role, string? ErrorMessage)> DeleteAsync(Role role)
        {
            await RepositoryWrapper.Roles.DeleteAsync(role);
            Role? newRole = await GetAllRolePermission(role);
            return (newRole, null);
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

        public async Task<Role?> GetAllRolePermission(Role role)
        {
            var newRolePermission = RepositoryWrapper.RolePermissions.FindByCondition(x => x.RoleId == role.Id);
            role.RolePermissions = await newRolePermission.ToListAsync();
            return role;
        }

        public async Task<Role?> GetByIdAsync(Guid roleId)
        {
            return await RepositoryWrapper.Roles.GetByIdAsync(roleId);
        }

        public async Task<(Role? Role, string? ErrorMessage)> GetDetailAsync(Guid roleId)
        {
            Role? role = await RepositoryWrapper.Roles.FindByCondition(x => x.Id == roleId).Include(x => x.RolePermissions).FirstOrDefaultAsync();
            if (role is null)
            {
                return (null, Messages.Roles.IdNotFound);
            }
            return (role, null);
        }

        public async Task<(Role? Role, string? ErrorMessage)> UpdateAsync(Role role, UpdateRoleRequest request)
        {
            //db role permission no tracking
            var entities = RepositoryWrapper.RolePermissions.FindByCondition(x => x.RoleId == role.Id, isAsNoTracking: true);

            //add role permisstion
            var addedRolePermissionsReq = request.RolePermissions!.Where(x => entities.All(v => v.RoleId == x.RoleId && v.Code != x.Code));
            var addedRolePermissions = Mapper.Map<List<RolePermission>>(addedRolePermissionsReq);
            //update role permisstion
            var updatedRolePermissionsReq = request.RolePermissions!.Where(x => entities.Any(v => v.RoleId == x.RoleId && v.Code == x.Code));
            var updatedRolePermissions = Mapper.Map<List<RolePermission>>(updatedRolePermissionsReq);

            //get deleted role permissions
            List<RolePermission> deletedRolePermissions = new();
            foreach (var update in updatedRolePermissions)
            {
                foreach (var entity in entities)
                {
                    if (entity.RoleId == update.RoleId && entity.Code != update.Code)
                    {
                        deletedRolePermissions.Add(entity);
                    }
                }
            }
            using var transaction = RepositoryWrapper.BeginTransactionAsync();
            {
                try
                {
                    foreach (var rolePermission in addedRolePermissions)
                    {
                        await RepositoryWrapper.RolePermissions.AddAsync(rolePermission);
                    }

                    foreach (var rolePermission in updatedRolePermissions)
                    {
                        await RepositoryWrapper.RolePermissions.UpdateAsync(rolePermission);
                    }

                    Mapper.Map(request, role);
                    await RepositoryWrapper.Roles.UpdateAsync(role);
                    await RepositoryWrapper.CommitTransactionAsync();

                    await RepositoryWrapper.RolePermissions.DeleteRangeAsync(deletedRolePermissions);
                }
                catch (Exception ex)
                {
                    await RepositoryWrapper.RollbackTransactionAsync();
                    Log.Error(ex, ex.GetBaseException().ToString());
                }
            }
            Role? newRole = await GetAllRolePermission(role);
            return (newRole, null);
        }
    }
}