using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.UserPermissionRequests;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.UserPermissionValidations
{
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