using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.RoleRequests;
using Infrastructure.Modules.Users.Validations.RolePermissionValidations;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.RolesValidations
{
    public class CreateRoleValidation : AbstractValidator<CreateRoleRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public CreateRoleValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.Roles.NameEmpty);
            RuleForEach(x => x.RolePermissions).Cascade(CascadeMode.Stop).SetValidator(new CreateRolePermissionValidation(RepositoryWrapper));
        }
    }
}