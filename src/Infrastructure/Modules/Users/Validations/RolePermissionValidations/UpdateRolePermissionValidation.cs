using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.RolePermissionRequests;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.RolePermissionValidations
{
    public class UpdateRolePermissionValidation : AbstractValidator<UpdateRolePermissionRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public UpdateRolePermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.RoleId)
                .NotNull().WithMessage(Messages.Roles.IdIsRequired)
                .MustAsync(async (roleId, cancellationToken) => await RepositoryWrapper.Roles!.IsAnyValue(x => x.Id == roleId))
                .WithMessage(Messages.Roles.IdNotFound);
            RuleFor(x => x.Code)
                .NotNull().WithMessage(Messages.Permissions.CodeIsRequired)
                .MustAsync(async (code, cancellationToken) => await RepositoryWrapper.Permissions!.IsAnyValue(x => x.Code == code))
                .WithMessage(Messages.Permissions.CodeNotFound);
        }
    }
}