using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Requests.UserRequets
{
    public class CreateRolePermissionRequest
    {
        public string? Code { get; set; }
    }
    public class CreateRolePermissionValidation : AbstractValidator<CreateRolePermissionRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public CreateRolePermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.Code)
                .NotNull().WithMessage(Messages.Permissions.CodeIsRequired)
                .MustAsync(async (code, cancellationToken) => await RepositoryWrapper.Permissions!.IsAnyValue(x => x.Code == code))
                .WithMessage(Messages.Permissions.CodeNotFound);
        }
    }
}