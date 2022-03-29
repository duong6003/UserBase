using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
namespace Infrastructure.Modules.Users.Requests.UserRequets
{
    public class DeleteUserPermissionRequest
    {
        public Guid UserId { get; set; }
        public string? Code { get; set; }
    }
    public class DeleteUserPermissionValidation : AbstractValidator<DeleteUserPermissionRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public DeleteUserPermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x)
                .MustAsync(async (per, cancellationToken)
                => await RepositoryWrapper.UserPermissions.IsAnyValue(x => x.UserId == per.UserId && x.Code == per.Code))
                .WithMessage(Messages.UserPermissions.UserPermissionNotExist);
        }
    }
}