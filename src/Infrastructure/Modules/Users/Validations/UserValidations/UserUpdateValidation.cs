using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests.UserRequests;
using Infrastructure.Modules.Users.Validations.UserPermissionValidations;
using Infrastructure.Persistence.GlobalValidation;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Infrastructure.Modules.Users.Validations.UserValidations
{
    public class UserUpdateValidation : AbstractValidator<UserUpdateRequest>
    {
        private readonly IActionContextAccessor ActionContextAccessor;
        private readonly IRepositoryWrapper RepositoryWrapper;

        public UserUpdateValidation(IRepositoryWrapper repositoryWrapper, IActionContextAccessor actionContextAccessor)
        {
            ActionContextAccessor = actionContextAccessor;
            Guid.TryParse(actionContextAccessor.ActionContext!.RouteData.Values
                .FirstOrDefault(x => x.Key.Equals("userId"))
                .Value?
                .ToString(), out Guid userId);
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.EmailAddress)
                .MustAsync(async(userReq, emailAddress, cancellationToken) => 
                !await RepositoryWrapper.Users.IsAnyValue(x => x.EmailAddress == emailAddress, userId))
                .WithMessage(Messages.Users.EmailAddressAlreadyExist)
                .EmailAddress().WithMessage(Messages.Users.EmailAddressInvalid);
            RuleFor(x => x.Password)
                .Length(6,128).WithMessage(Messages.Users.PasswordLengthInvalid + "Equal{MinLength}To{MaxLength}")
                .Matches("[a-zA-Z0-9_]{0,23}").WithMessage(Messages.Users.PasswordInvalid);
            RuleFor(x => x.ConfirmPassword).Equal(x=> x.Password).WithMessage(Messages.Users.PasswordConfirmNotMatch);
            RuleFor(x => x.Avatar)
                .IsValidFile("image").WithMessage(Messages.Users.UserAvatarInValid);
            RuleFor(x => x.RoleId)
                .NotNull().WithMessage(Messages.Roles.IdIsRequired)
                .MustAsync( async(roleId, cancellationToken) => await RepositoryWrapper.Roles.IsAnyValue(x=> x.Id == roleId))
                .WithMessage(Messages.Roles.IdNotFound);
        }
    }
}