using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.UserRequests;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.UserValidations
{
    public class ForgotPasswordValidation : AbstractValidator<ForgotPasswordRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public ForgotPasswordValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
        
            RuleFor(x => x.Email)
                .MustAsync(async(email, cancellationToken) => await RepositoryWrapper.Users.IsExistProperty(x=> x.EmailAddress == email)).WithMessage(Messages.Users.UserNameNotExist);
        }
    }
}