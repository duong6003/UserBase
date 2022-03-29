using AutoMapper;
using Envelop.App.Ultilities;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.UserRequets;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Services
{
    public interface IPermissionService
    {
        Task<Permission?> GetByIdAsync(Guid permissionId);

        Task<(PaginationResponse<Permission>, string? ErrorMessage)> GetAllAsync(PaginationRequest request);

        Task<(Permission? Permission, string? ErrorMessage)> CreateAsync(CreatePermissionRequest request);

        Task<(Permission? Permission, string? ErrorMessage)> UpdateAsync(Permission permission, UpdatePermissionRequest request);

        Task<(Permission? Permission, string? ErrorMessage)> DeleteAsync(Permission permission);
    }

    public class PermissionService : IPermissionService
    {
        private readonly IRepositoryWrapper RepositoryWrapper;
        private readonly IMapper Mapper;

        public PermissionService(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            RepositoryWrapper = repositoryWrapper;
            Mapper = mapper;
        }

        public async Task<(Permission? Permission, string? ErrorMessage)> CreateAsync(CreatePermissionRequest request)
        {
            Permission? permission = Mapper.Map<Permission>(request);
            await RepositoryWrapper.Permissions.AddAsync(permission);
            return (permission, null);
        }

        public async Task<(Permission? Permission, string? ErrorMessage)> DeleteAsync(Permission permission)
        {
            await RepositoryWrapper.Permissions.DeleteAsync(permission);
            return (permission, null);
        }

        public async Task<(PaginationResponse<Permission>, string? ErrorMessage)> GetAllAsync(PaginationRequest request)
        {
            IQueryable<Permission>? permissions = RepositoryWrapper.Permissions.FindByCondition(x =>
                (
                    string.IsNullOrEmpty(request.SearchContent)
                    || x.Name!.ToLower().Contains(request.SearchContent!.ToLower())
                    || x.Code!.ToLower().Contains(request.SearchContent!.ToLower())
                ));
            permissions = SortUtility<Permission>.ApplySort(permissions, request.OrderByQuery!);
            PaginationUtility<Permission>? data = await PaginationUtility<Permission>.ToPagedListAsync(permissions, request.PageNumber, request.PageSize);
            return (PaginationResponse<Permission>.PaginationInfo(data, data.PageInfo), Messages.Permissions.GetAllSuccessfully);
        }

        public async Task<Permission?> GetByIdAsync(Guid permissionId)
        {
            return await RepositoryWrapper.Permissions.GetByIdAsync(permissionId);
        }

        public async Task<(Permission? Permission, string? ErrorMessage)> UpdateAsync(Permission permission, UpdatePermissionRequest request)
        {
            Mapper.Map(request, permission);
            await RepositoryWrapper.Permissions.UpdateAsync(permission);
            return (permission, null);
        }
    }
}