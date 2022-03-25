using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.UserRequests;
using Infrastructure.Modules.Users.Validations.UserPermissionValidations;
using Infrastructure.Persistence.GlobalValidation;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.UserValidations
{
    public class UserUpdateValidation : AbstractValidator<UserUpdateRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public UserUpdateValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.EmailAddress)
                .MustAsync(async(emailAddress, cancellationToken) => !await RepositoryWrapper.Users.IsExistProperty(x => x.EmailAddress == emailAddress)).WithMessage(Messages.Users.EmailAddressAlreadyExist)
                .EmailAddress().WithMessage(Messages.Users.EmailAddressInvalid);
            RuleFor(x => x.Password)
                .Length(6,128).WithMessage(Messages.Users.PasswordLengthInvalid + "Equal{MinLength}To{MaxLength}")
                .Matches("[a-zA-Z0-9_]{0,23}").WithMessage(Messages.Users.PasswordInvalid);
            RuleFor(x => x.ConfirmPassword).Equal(x=> x.Password).WithMessage(Messages.Users.PasswordConfirmNotMatch);
            RuleFor(x => x.Avatar)
                .IsValidFile("image");
            RuleFor(x => x.RoleId)
                .NotNull().WithMessage(Messages.Roles.IdIsRequired)
                .MustAsync( async(roleId, cancellationToken) => await RepositoryWrapper.Roles.IsExistId(roleId)).WithMessage(Messages.Roles.IdNotFound);
            RuleForEach(x => x.UserPermissions).Cascade(CascadeMode.Stop).SetValidator(new UpdateUserPermissionValidation(RepositoryWrapper));
        }
    }
}