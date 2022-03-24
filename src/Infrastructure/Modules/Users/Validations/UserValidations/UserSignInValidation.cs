using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.UserRequests;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.UserValidations
{
    public class UserSignInValidation : AbstractValidator<UserSignInRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public UserSignInValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.UserName)
                .MustAsync(async(userName, cancellationToken) => await RepositoryWrapper.Users.IsExistProperty(x => x.UserName == userName)).WithMessage(Messages.Users.UserNameNotExist)
                .Matches(@"^(?=[a-zA-Z])[-\w.]{0,23}([a-zA-Z\d]|(?<![-.])_)$").WithMessage(Messages.Users.UserNameInvalid)
                .MaximumLength(50).WithMessage(Messages.Users.UserNameInValidLength + "{MaxLength}");

            RuleFor(x => x.Password)
                .Length(6,128).WithMessage(Messages.Users.PasswordLengthInvalid + "Equal{MinLength}To{MaxLength}")
                .Matches("[a-zA-Z0-9_]{0,23}").WithMessage(Messages.Users.PasswordInvalid);
        }
    }
}