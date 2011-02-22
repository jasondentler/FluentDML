using System;
using System.Data;
using System.Linq.Expressions;

namespace FluentDML
{

    public interface IInsertSet<T>
    {
        IInsertSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);
        IDbCommand ToCommand();
    }

}
