using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.UserPermissionRequests;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.UserPermissionValidations
{
    public class CreateUserPermissionValidation : AbstractValidator<CreateUserPermission>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public CreateUserPermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.UserId)
                .NotNull().WithMessage(Messages.Users.IdIsRequired)
                .MustAsync(async(userId, cancellationToken) => await RepositoryWrapper.Users!.IsExistId(userId))
                .WithMessage(Messages.Users.IdNotFound);
            RuleFor(x => x.PermissionId)
                .NotNull().WithMessage(Messages.Permissions.IdIsRequired)
                .MustAsync(async(permissionId, obj) => await RepositoryWrapper.Permissions!.IsExistId(permissionId))
                .WithMessage(Messages.Permissions.IdNotFound);
        }
    }
}