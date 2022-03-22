using System.Linq.Expressions;

namespace Core.Common.Validation
{
    public interface IGlobalValidation<TEntity, Tid> where TEntity : class
    {
        bool IsExistId(Tid id);
    }
}
