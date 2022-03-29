using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.PermissionRequests;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.PermissionValidations
{
    public class UpdatePermissionValidation : AbstractValidator<UpdatePermissionRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public UpdatePermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.Name).NotEmpty().WithMessage(Messages.Permissions.NameEmpty);
        }
    }
}