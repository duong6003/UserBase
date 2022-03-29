using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
namespace Infrastructure.Modules.Users.Requests.UserRequets
{
    public class ForgotPasswordRequest
    {
        public string? Email { get; set; }
    }
    public class ForgotPasswordValidation : AbstractValidator<ForgotPasswordRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public ForgotPasswordValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;

            RuleFor(x => x.Email)
                .MustAsync(async (email, cancellationToken) => await RepositoryWrapper.Users.IsAnyValue(x => x.EmailAddress == email)).WithMessage(Messages.Users.UserNameNotExist);
        }
    }
}