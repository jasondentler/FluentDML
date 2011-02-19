using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentDML.Mapping
{
    public class ClassMap
    {
        private readonly Type _mappedType;
        private readonly string _tableName;
        private readonly Dictionary<string, string> _propertyMap;

        public ClassMap(Type mappedType, string tableName)
        {
            MappedType = mappedType;
            TableName = tableName;
            _propertyMap = new Dictionary<string, string>();
        }

        public Type MappedType { get; private set; }
        public string TableName { get; private set; }

        public void Add<T>(Expression<Func<T, object>> property, string columnName)
        {
            var propertyPath = FindMemberVisitor.FindMember(property);
            Add(propertyPath, columnName);
        }

        public void Add(string propertyPath, string columnName)
        {
            _propertyMap.Add(propertyPath, columnName);
        }

        public string GetColumnName<T>(Expression<Func<T, object>> property)
        {
            var propertyPath = FindMemberVisitor.FindMember(property);
            return GetColumnName(propertyPath);
        }

        public string GetColumnName(string propertyPath)
        {
            return _propertyMap[propertyPath];
        }

    }
}
