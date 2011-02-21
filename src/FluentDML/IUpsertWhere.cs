using System;
using System.Data;
using System.Linq.Expressions;

namespace FluentDML
{
    public interface IUpsertWhere<T>
    {
        IUpsertWhere<T> And(Expression<Func<T, bool>> predicate);
        IDbCommand ToCommand();
    }
}