using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Requests.UserRequets
{
    public class UpdatePermissionRequest
    {
        public string? Name { get; set; }
    }
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