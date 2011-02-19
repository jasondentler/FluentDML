using System;
using System.Collections.Generic;

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

    }
}
