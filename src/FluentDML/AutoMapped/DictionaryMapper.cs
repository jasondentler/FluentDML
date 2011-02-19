using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentDML.Mapping;

namespace FluentDML.AutoMapped
{

    public class DictionaryMapper
    {

        public static Dictionary<string, object> GetValueMap<TSource, TDestination>(TSource source, Map map)
        {
            return GetValueMap(typeof (TSource), typeof (TDestination), source, map);
        }

        public static Dictionary<string, object> GetValueMap(Type sourceType, Type destinationType, object source, Map map)
        {
            var classMap = map.GetClassMap(destinationType);
            var valueMap = new Dictionary<string, object>();
            new DictionaryMapper(sourceType, destinationType, source, classMap, "", valueMap).Process();
            return valueMap;
        }

        public static Dictionary<string, object> GetValueMap<TSource, TDestination>(TSource source, ClassMap classMap)
        {
            var valueMap = new Dictionary<string, object>();
            new DictionaryMapper(typeof (TSource), typeof (TDestination), source, classMap, "", valueMap).Process();
            return valueMap;
        }

        private readonly Type _sourceType;
        private readonly Type _destinationType;
        private readonly object _source;
        private readonly ClassMap _classMap;
        private readonly string _propertyPath;
        private readonly TypeMap _map;
        private readonly ResolutionContext _context;
        private readonly Dictionary<string, object> _values;

        private DictionaryMapper(
            Type sourceType, Type destinationType, object source, ClassMap classMap,
            string propertyPath, Dictionary<string, object > values)
        {
            _sourceType = sourceType;
            _destinationType = destinationType;
            _source = source;
            _classMap = classMap;
            _propertyPath = propertyPath;
            _values = values;
            _map = Mapper.FindTypeMapFor(sourceType, destinationType);
            _context = new ResolutionContext(_map, _source, _sourceType, _destinationType);
        }

        private void Process()
        {

            var propertyMaps = _map.GetPropertyMaps()
                .Where(pm => pm.IsMapped() && !pm.IsIgnored() && pm.ShouldAssignValue(_context));
            foreach (var propMap in propertyMaps)
            {
                var path = GetPropertyPath(propMap);
                var result = propMap.ResolveValue(_context);
                var value = result.Value;
                if (IsComponent(propMap))
                {
                    new DictionaryMapper(
                        result.MemberType, propMap.DestinationProperty.MemberType,
                        value, _classMap, path, _values).Process();
                }
                else
                {
                    _values.Add(GetPropertyPath(propMap), value);
                }
            }
        }

        private bool IsComponent(PropertyMap propertyMap)
        {
            return _classMap.IsComponent(GetPropertyPath(propertyMap));
        }

        private string GetPropertyPath(PropertyMap propertyMap)
        {
            var propertyName = propertyMap.DestinationProperty.Name;
            if (string.IsNullOrEmpty(_propertyPath))
                return propertyName;
            return string.Join(".", new[] {_propertyPath, propertyName});
        }


    }

}
