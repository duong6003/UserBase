using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.GlobalValidation;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Modules.Users.Requests.UserRequests
{
    public class UserSignUpRequest
    {
        public string? UserName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public IFormFile? Avatar { get; set; }
        public Guid? RoleId { get; set; }
    }
    public class UserSignUpValidation : AbstractValidator<UserSignUpRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public UserSignUpValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(Messages.Users.UserNameEmpty)
                .MustAsync(async (userName, cancellationToken) => !await RepositoryWrapper.Users.IsAnyValue(x => x.UserName == userName)).WithMessage(Messages.Users.UserNameAlreadyExist)
                .Matches(@"^(?=[a-zA-Z])[-\w.]{0,23}([a-zA-Z\d]|(?<![-.])_)$").WithMessage(Messages.Users.UserNameInvalid)
                .MaximumLength(50).WithMessage(Messages.Users.UserNameInValidLength + "{MaxLength}");
            RuleFor(x => x.EmailAddress)
                .MustAsync(async (emailAddress, cancellationToken) => !await RepositoryWrapper.Users.IsAnyValue(x => x.EmailAddress == emailAddress)).WithMessage(Messages.Users.EmailAddressAlreadyExist)
                .EmailAddress().WithMessage(Messages.Users.EmailAddressInvalid);
            RuleFor(x => x.Password)
                .Length(6, 128).WithMessage(Messages.Users.PasswordLengthInvalid + "Equal{MinLength}To{MaxLength}")
                .Matches("[a-zA-Z0-9_]{0,23}").WithMessage(Messages.Users.PasswordInvalid);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(Messages.Users.PasswordConfirmNotMatch);
            RuleFor(x => x.Avatar)
                .IsValidFile("image").WithMessage(Messages.Users.UserAvatarInValid);
            RuleFor(x => x.RoleId)
                .NotNull().WithMessage(Messages.Roles.IdIsRequired)
                .MustAsync(async (roleId, cancellationToken) => await RepositoryWrapper.Roles.IsAnyValue(x => x.Id == roleId)).WithMessage(Messages.Roles.IdNotFound);
        }
    }
}