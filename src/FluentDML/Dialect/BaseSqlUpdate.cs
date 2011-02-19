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
    public abstract class BaseSqlUpdate<T> :
        IUpdate<T>, 
        IUpdateSet<T>,
        IUpdateWhere<T>
    {

        private readonly Dictionary<string, object> _setMap;
        private readonly List<MyExpression> _predicates;
        private readonly ClassMap _map;

        protected BaseSqlUpdate(ClassMap map)
        {
            _map = map;
            _setMap = new Dictionary<string, object>();
            _predicates = new List<MyExpression>();
        }

        public IUpdateSet<T> Set<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            _setMap.Add(_map.GetColumnName(property), value);
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
            return ToCommand(_map.TableName, _setMap, And(_predicates));
        }

        protected virtual MyExpression And(IEnumerable<MyExpression> predicatesEnum)
        {
            if (!predicatesEnum.Any())
                return null;
            var q = new Queue<MyExpression>(predicatesEnum);
            var root = q.Dequeue();
            while (q.Any())
                root = new Binary(root, q.Dequeue(), ExpressionType.AndAlso);
            return root;
        }

        protected abstract IDbCommand ToCommand(
            string tableName,
            Dictionary<string, object> set,
            MyExpression predicate);

    }
}
