using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentDML.Mapping
{
    public class Map
    {

        private readonly Dictionary<Type, ClassMap> _classMaps;

        public Map()
        {
            _classMaps = new Dictionary<Type, ClassMap>();
        }

        public void Add(ClassMap classMap)
        {
            _classMaps.Add(classMap.MappedType, classMap);
        }

        public ClassMap GetClassMap(Type type)
        {
            return _classMaps[type];
        }

        public ClassMap GetClassMap<T>()
        {
            return GetClassMap(typeof (T));
        }

        public ClassMap GetClassMap<T>(T instance)
        {
            return GetClassMap<T>();
        }

        public string GetColumnName<T, TProperty>(Expression<Func<T, TProperty>> property)
        {
            return GetClassMap<T>().GetColumnName(property);
        }

    }
}
