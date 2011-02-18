using System;
using System.Data;
using System.Linq.Expressions;

namespace FluentDML
{
    public interface IDeleteWhere<T>
    {

        IDeleteWhere<T> And(Expression<Func<T, bool>> predicate);
        IDbCommand ToCommand();

    }
}
