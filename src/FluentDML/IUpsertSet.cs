using System;
using System.Linq.Expressions;

namespace FluentDML
{
    public interface IUpsertSet<T> : IUpsertWhere<T>
    {

        IUpsertSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

    }
}
