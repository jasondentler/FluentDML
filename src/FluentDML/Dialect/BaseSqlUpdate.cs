using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using FluentDML.Expressions;
using FluentDML.Expressions.AST;
using FluentDML.Mapping;

namespace FluentDML.Dialect
{
    public abstract class BaseSqlUpdate<T> : BaseSql<T>,
        IUpdate<T>, 
        IUpdateSet<T>,
        IUpdateWhere<T>
    {

        private readonly Dictionary<string, object> _setMap;
        private readonly List<SimpleExpression> _predicates;

        protected BaseSqlUpdate(ClassMap map)
            : base(map)
        {
            _setMap = new Dictionary<string, object>();
            _predicates = new List<SimpleExpression>();
        }

        public IUpdateSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            _setMap.Add(Map.GetColumnName(property), value);
            return this;
        }

        public IUpdateWhere<T> Where(Expression<Func<T, bool>> predicate)
        {
            _predicates.Add(PredicateParserVisitor.Parse(predicate));
            return this;
        }

        public IUpdateWhere<T> And(Expression<Func<T, bool>> predicate)
        {
            _predicates.Add(PredicateParserVisitor.Parse(predicate));
            return this;
        }

        public IDbCommand ToCommand()
        {
            return ToCommand(Map.TableName, _setMap, And(_predicates));
        }

        protected abstract IDbCommand ToCommand(
            string tableName,
            Dictionary<string, object> set,
            SimpleExpression predicate);

    }
}
