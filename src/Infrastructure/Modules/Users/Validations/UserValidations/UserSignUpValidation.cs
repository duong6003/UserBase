using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.UserRequests;
using Infrastructure.Persistence.GlobalValidation;

namespace Infrastructure.Modules.Users.Validations.UserValidations
{
    public class UserSignUpValidation : AbstractValidator<UserSignUpRequest>
    {
        private readonly GlobalUserValidation _userValidation;

        public UserSignUpValidation(GlobalUserValidation userValidation)
        {
            _userValidation = userValidation;
            RuleFor(x => x.UserName)
                .Must((req, userName) => !_userValidation.IsExistProperty(x => x.UserName == userName)).WithMessage(Messages.Users.UserNameAlreadyExist)
                .Matches(@"^(?=[a-zA-Z])[-\w.]{0,23}([a-zA-Z\d]|(?<![-.])_)$").WithMessage(Messages.Users.UserNameInvalid)
                .MaximumLength(50).WithMessage(Messages.Users.UserNameInValidLength + "{MaxLength}");
            RuleFor(x => x.EmailAddress)
                .Must((req, emailAddress) => !_userValidation.IsExistProperty(x => x.EmailAddress == emailAddress)).WithMessage(Messages.Users.EmailAddressAlreadyExist)
                .EmailAddress().WithMessage(Messages.Users.EmailAddressInvalid);
            RuleFor(x => x.Password)
                .Length(6,128).WithMessage(Messages.Users.PasswordLengthInvalid + "Equal{MinLength}To{MaxLength}")
                .Matches("[a-zA-Z0-9_]{0,23}").WithMessage(Messages.Users.PasswordInvalid);
            RuleFor(x => x.ConfirmPassword).Equal(x=> x.Password).WithMessage(Messages.Users.PasswordConfirmNotMatch);
            RuleFor(x => x.Avatar)
                .IsValidFile("image");
        }
    }
}