using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.UserRequests;

namespace Infrastructure.Modules.Users.Validations.UserValidations
{
    public class ForgotPasswordValidation : AbstractValidator<ForgotPasswordRequest>
    {
        private readonly GlobalUserValidation _userValidation;

        public ForgotPasswordValidation(GlobalUserValidation userValidation)
        {
            _userValidation = userValidation;
            RuleFor(x => x.UserName)
                .Must((req, userName) => _userValidation.IsExistProperty(x=> x.UserName == userName)).WithMessage(Messages.Users.UserNameNotFound);
        }
    }
}