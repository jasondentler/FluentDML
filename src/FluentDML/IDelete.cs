using System;
using System.Data;
using System.Linq.Expressions;

namespace FluentDML
{
    public interface IDelete<T>
    {

        IDbCommand Where(Expression<Func<T, bool>> predicate);

    }
}
