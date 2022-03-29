using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
namespace Infrastructure.Modules.Users.Requests.UserRequets
{
    public class CreateUserPermissionRequest
    {
        public Guid UserId { get; set; }
        public string? Code { get; set; }
    }
    public class CreateUserPermissionValidation : AbstractValidator<CreateUserPermissionRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public CreateUserPermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x)
                .MustAsync(async (per, cancellationToken)
                => !await RepositoryWrapper.UserPermissions.IsAnyValue(x => x.UserId == per.UserId && x.Code == per.Code))
                .WithMessage(Messages.UserPermissions.UserPermissionIsExisted);
            RuleFor(x => x.UserId)
                .NotNull().WithMessage(Messages.Users.IdIsRequired)
                .MustAsync(async (userId, cancellationToken) => await RepositoryWrapper.Users!.IsAnyValue(x => x.Id == userId))
                .WithMessage(Messages.Users.IdNotFound);
            RuleFor(x => x.Code)
                .NotNull().WithMessage(Messages.Permissions.CodeIsRequired)
                .MustAsync(async (code, obj) => await RepositoryWrapper.Permissions!.IsAnyValue(x => x.Code == code))
                .WithMessage(Messages.Permissions.CodeNotFound);
        }
    }
}