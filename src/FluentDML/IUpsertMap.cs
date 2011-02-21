using System;
using System.Data;
using System.Linq.Expressions;

namespace FluentDML
{
    public interface IUpsertMap<T>
    {
        IDbCommand WithId<TProperty>(Expression<Func<T, TProperty>> property);
    }
}