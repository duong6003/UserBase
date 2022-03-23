using System.Linq.Expressions;

namespace Core.Common.Validation
{
    public interface IGlobalValidation<TEntity> where TEntity : class
    {
        Task<bool> IsExistId<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
        bool IsExistProperty(Expression<Func<TEntity, bool>> expression);
    }
}
