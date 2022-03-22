using Core.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Validation
{
    public interface IGlobalValidation<TEntity, Tid> where TEntity : class
    {
        bool IsExistId(Tid id, Expression<Action<TEntity>> preticate);
        bool IsValidFileName(string fileName);
    }
}
