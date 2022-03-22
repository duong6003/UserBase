using Infrastructure.Modules.Users.Entities;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence.Repositories;

public interface IRepositoryWrapper
{
    IRepositoryBase<User> Users { get; }

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly ApplicationDbContext ApplicationDbContext;

    private IRepositoryBase<User>? UsersRepositoryBase;

    public RepositoryWrapper(ApplicationDbContext applicationDbContext) => ApplicationDbContext = applicationDbContext;

    public IRepositoryBase<User> Users => UsersRepositoryBase ??= new RepositoryBase<User>(ApplicationDbContext);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) => await ApplicationDbContext.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) => await ApplicationDbContext.Database.CommitTransactionAsync(cancellationToken);

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => await ApplicationDbContext.Database.RollbackTransactionAsync(cancellationToken);
}
