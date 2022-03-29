using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.PermissionRequests;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.PermissionValidations
{
    public class CreatePermissionValidation : AbstractValidator<CreatePermissionRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public CreatePermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(Messages.Permissions.CodeEmpty)
                .MustAsync(async(code, cancellationToken) => !await RepositoryWrapper.Permissions.IsAnyValue(x=> x.Code == code))
                .WithMessage(Messages.Permissions.CodeExisted);
            RuleFor(x => x.Name).NotEmpty().WithMessage(Messages.Permissions.NameEmpty);
        }
    }
}