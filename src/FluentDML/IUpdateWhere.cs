using System;
using System.Data;
using System.Linq.Expressions;

namespace FluentDML
{
    public interface IUpdateWhere<T>
    {

        IUpdateWhere<T> And(Expression<Func<T, bool>> predicate);
        IDbCommand ToCommand();

    }
}
