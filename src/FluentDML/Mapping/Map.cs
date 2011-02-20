using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FluentDML.Mapping
{
    public class Map
    {

        private readonly ConcurrentDictionary<Type, ClassMap> _classMaps;

        public Map()
        {
            _classMaps = new ConcurrentDictionary<Type, ClassMap>();
        }

        public void Add(ClassMap classMap)
        {
            _classMaps.AddOrUpdate(classMap.MappedType, classMap,
                                   (t, newClassMap) => newClassMap);
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
