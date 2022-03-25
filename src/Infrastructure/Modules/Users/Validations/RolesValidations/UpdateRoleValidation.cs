using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.RoleRequests;
using Infrastructure.Modules.Users.Validations.RolePermissionValidations;
using Infrastructure.Persistence.GlobalValidation;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.RolesValidations
{
    public class UpdateRoleValidation : AbstractValidator<UpdateRoleRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;
        public UpdateRoleValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.Roles.NameEmpty)
                .IsValidVietNamName().WithMessage(Messages.Roles.NameInValid);
            RuleForEach(x => x.RolePermissions).Cascade(CascadeMode.Stop).SetValidator(new UpdateRolePermissionValidation(RepositoryWrapper));
        }
    }
}