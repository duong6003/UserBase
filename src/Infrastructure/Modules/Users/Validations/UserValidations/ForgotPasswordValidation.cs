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
        
            RuleFor(x => x.UserName)
                .MustAsync(async(userName, cancellationToken) => await RepositoryWrapper.Users.IsExistProperty(x=> x.UserName == userName)).WithMessage(Messages.Users.UserNameNotExist);
        }
    }
}