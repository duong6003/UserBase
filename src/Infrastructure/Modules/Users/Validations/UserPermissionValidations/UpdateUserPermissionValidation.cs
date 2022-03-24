using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Requests.UserPermissionRequests;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Validations.UserPermissionValidations
{
    public class UpdateUserPermissionValidation : AbstractValidator<UpdateUserPermissionRequest>
    {
        private readonly IRepositoryWrapper RepositoryWrapper;

        public UpdateUserPermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
            RuleFor(x => x.UserId)
                .NotNull().WithMessage(Messages.Users.IdIsRequired)
                .MustAsync(async(userId, cancellationToken) => await RepositoryWrapper.Users!.IsExistId(userId))
                .WithMessage(Messages.Users.IdNotFound);
            RuleFor(x => x.PermissionId)
                .NotNull().WithMessage(Messages.Permissions.IdIsRequired)
                .MustAsync(async(permissionId, obj) => await RepositoryWrapper.Permissions!.IsExistId(permissionId))
                .WithMessage(Messages.Permissions.IdNotFound);
        }
    }
}