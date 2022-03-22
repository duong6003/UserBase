using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid userId);
    Task<(User? User, string? ErrorMessage)> GetDetailAsync(Guid userId);
}

public class UserService : IUserService
{
    private readonly IRepositoryWrapper RepositoryWrapper;

    public UserService(IRepositoryWrapper repositoryWrapper)
    {
        RepositoryWrapper = repositoryWrapper;
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await RepositoryWrapper.Users.GetByIdAsync(userId);
    }

    public async Task<(User? User, string? ErrorMessage)> GetDetailAsync(Guid userId)
    {
        User? user = await GetByIdAsync(userId);
        if (user is null)
        {
            return (null, Messages.Users.IdNotFound);
        }
        return (user, null);
    }
}
