using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FluentDML.Mapping
{
    public abstract class MapMaker : IMapMaker
    {

        private List<Type> _typesToMap = new List<Type>();

        public Map MakeMap(Assembly assembly)
        {
            return MakeMap(assembly, t => t.IsClass && !t.IsAbstract);
        }

        public Map MakeMap(Assembly assembly, Predicate<Type> filter)
        {
            return MakeMap(assembly.GetTypes().Where(t => filter(t)));
        }

        public Map MakeMap(params Type[] types)
        {
            return MakeMap(types.AsEnumerable());
        }

        public Map MakeMap(IEnumerable<Type> types)
        {
            _typesToMap.AddRange(types);
            return MakeMap();
        }

        public Map MakeMap()
        {
            var map = new Map();
            foreach (var type in _typesToMap)
                map.Add(MakeClassMap(type));
            return map;
        }

        protected abstract ClassMap MakeClassMap(Type type);

    }
}
