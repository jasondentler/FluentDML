using System.Data;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public abstract class BaseSqlInsert<T> : BaseSql<T>, IInsert<T>
    {
        protected BaseSqlInsert(ClassMap map) : base(map)
        {
        }

        public abstract IDbCommand MapFrom<TSource>(TSource source);
    }
}
