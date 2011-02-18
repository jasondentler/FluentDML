using System;
using System.Data;
using System.Linq.Expressions;

namespace FluentDML
{
    public interface IDelete<T>
    {

        IDeleteWhere<T> Where(Expression<Func<T, bool>> predicate);

    }
}
