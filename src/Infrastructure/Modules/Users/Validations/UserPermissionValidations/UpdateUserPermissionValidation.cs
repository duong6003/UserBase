using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.UserPermissionRequests;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.UserPermissionValidations
{
    public class UpdateUserPermissionValidation : AbstractValidator<UpdateUserPermissionRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public UpdateUserPermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.UserId)
                .NotNull().WithMessage(Messages.Users.IdIsRequired)
                .MustAsync(async(userId, cancellationToken) => await RepositoryWrapper.Users!.IsAnyValue(x => x.Id == userId))
                .WithMessage(Messages.Users.IdNotFound);
            RuleFor(x => x.Code)
                .NotNull().WithMessage(Messages.Permissions.CodeIsRequired)
                .MustAsync(async(code, obj) => await RepositoryWrapper.Permissions!.IsAnyValue(x=> x.Code == code))
                .WithMessage(Messages.Permissions.CodeNotFound);
        }
    }
}