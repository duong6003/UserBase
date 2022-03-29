using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
namespace Infrastructure.Modules.Users.Requests.UserRequets
{
    public class UpdateRoleRequest
    {
        public string? Name { get; set; }
        public IList<UpdateRolePermissionRequest>? RolePermissions { get; set; }
    }
    public class UpdateRoleValidation : AbstractValidator<UpdateRoleRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public UpdateRoleValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(Messages.Roles.NameEmpty);
            RuleForEach(x => x.RolePermissions).Cascade(CascadeMode.Stop).SetValidator(new UpdateRolePermissionValidation(RepositoryWrapper));
        }
    }
}