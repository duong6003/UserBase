using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.RoleRequests;
using Infrastructure.Modules.Users.Validations.RolePermissionValidations;
using Infrastructure.Persistence.GlobalValidation;
using Infrastructure.Persistence.Repositories;
using static Infrastructure.Definitions.Messages;

namespace Infrastructure.Modules.Users.Validations.RolesValidations
{
    public class CreateRoleValidation : AbstractValidator<CreateRoleRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;


        public CreateRoleValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;

            RuleFor(x => x.Name)
                .Empty().WithMessage(Messages.Roles.NameEmpty)
                .IsValidVietNamName().WithMessage(Messages.Roles.NameInValid);
            RuleForEach(x => x.RolePermissions).Cascade(CascadeMode.Stop).SetValidator(new CreateRolePermissionValidation(RepositoryWrapper));
        }
    }
}