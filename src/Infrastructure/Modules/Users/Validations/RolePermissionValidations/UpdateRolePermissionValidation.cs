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
                .Must(roleId => RepositoryWrapper.RolePermissions!.IsExistProperty(x => x.RoleId == roleId))
                .WithMessage(Messages.Roles.IdNotFound);
            RuleFor(x => x.PermissionId)
                .NotNull().WithMessage(Messages.Permissions.IdIsRequired)
                .Must(permissionId => RepositoryWrapper.Permissions!.IsExistProperty(x => x.Id == permissionId))
                .WithMessage(Messages.Permissions.IdNotFound);
        }
    }
}