using AutoMapper;
using Envelop.App.Ultilities;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Services
{
    public interface IPermissionService
    {
        Task<(PaginationResponse<Permission>, string? ErrorMessage)> GetAllAsync(PaginationRequest request);
    }
    public class PermissionService : IPermissionService
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public PermissionService(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
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
    }
}