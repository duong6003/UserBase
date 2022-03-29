
using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Requests.UserRequets
{
    public class CreateRoleRequest
    {
        public string? Name { get; set; }
        public ICollection<CreateRolePermissionRequest>? RolePermissions { get; set; }
    }
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