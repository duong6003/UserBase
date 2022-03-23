using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests;

namespace Infrastructure.Modules.Users.Validations
{
    public class UserSignInValidation : AbstractValidator<UserSignInRequest>
    {
        private readonly GlobalUserValidation _userValidation;

        public UserSignInValidation(GlobalUserValidation userValidation)
        {
            _userValidation = userValidation;
            RuleFor(x => x.UserName)
                .Must((req, userName) => !_userValidation.IsExistProperty(x => x.UserName == userName)).WithMessage(Messages.Users.UserNameAlreadyExist)
                .Matches(@"^(?=[a-zA-Z])[-\w.]{0,23}([a-zA-Z\d]|(?<![-.])_)$").WithMessage(Messages.Users.UserNameInvalid)
                .MaximumLength(50).WithMessage(Messages.Users.UserNameInValidLength + "{MaxLength}");

            RuleFor(x => x.Password)
                .Length(6,128).WithMessage(Messages.Users.PasswordLengthInvalid + "Equal{MinLength}To{MaxLength}")
                .Matches("[a-zA-Z0-9_]{0,23}").WithMessage(Messages.Users.PasswordInvalid);
        }
    }
}