using System;
using System.Collections.Generic;
using System.Linq;
using FluentDML.Mapping;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using NHibernate.Type;
using Map = FluentDML.Mapping.Map;

namespace FluentDML.NHibernateAdapter
{
    public class NHibernateMapMaker : IMapMaker
    {
        private readonly Configuration _configuration;
        private IMapping _mapping;

        public NHibernateMapMaker(Configuration configuration)
        {
            _configuration = configuration;
        }

        public Map MakeMap()
        {
            if (_mapping == null)
                _mapping = _configuration.BuildMapping();
            var map = new Map();
            foreach (var classMap in BuildMappings())
                map.Add(classMap);
            return map;
        }

        protected virtual IEnumerable<ClassMap> BuildMappings()
        {
            return BuildMappings(_configuration.ClassMappings);
        }

        protected virtual IEnumerable<ClassMap> BuildMappings(IEnumerable<PersistentClass> persistentClasses)
        {
            return persistentClasses.Select(BuildMapping);
        }

        protected virtual ClassMap BuildMapping(PersistentClass persistentClass)
        {

            var classType = persistentClass.MappedClass;
            var tableName = persistentClass.Table.Name;
            var map = new ClassMap(classType, tableName);
            AddIdentifier(map, persistentClass);
            AddProperties(map, persistentClass);
            return map;
        }

        protected virtual void AddIdentifier(ClassMap map, PersistentClass persistentClass)
        {
            if (persistentClass.HasIdentifierProperty)
                AddProperty(map, persistentClass.IdentifierProperty);
        }

        protected virtual void AddProperties(ClassMap map, PersistentClass persistentClass)
        {
            foreach (var property in persistentClass.PropertyIterator)
                AddProperty(map, property);
        }

        protected virtual void AddProperty(ClassMap map, Property property)
        {
            if (property == null) return;

            if (property.Type.IsComponentType)
            {
                AddComponent(map, property, (ComponentType) property.Type);
                return;
            }

            var name = property.Name;
            if (property.ColumnSpan != 1)
                throw new NotSupportedException(
                    string.Format(
                        "The multi-column property {0}.{1} is not supported. Each property must map to a single column",
                        property.PersistentClass.ClassName,
                        property.Name));

            var column = property.ColumnIterator.Single().Text;
            map.Add(name, column);
        }

        protected virtual void AddComponent(ClassMap map, Property property, ComponentType componentType)
        {
            // Assuming componentType.PropertyNames match up to property.ColumnIterator... 
            for (var propertyIndex = 0; propertyIndex < componentType.PropertyNames.Length; propertyIndex++)
            {
                var columnCount = componentType.Subtypes[propertyIndex].GetColumnSpan(_mapping);
                if (columnCount != 1)
                    throw new NotSupportedException(
                        string.Format(
                            "The multi-column property {0}.{1}.{2} is not supported. Each property must map to a single column",
                            property.PersistentClass.ClassName,
                            property.Name,
                            componentType.PropertyNames[propertyIndex]));
                var propertyPath = property.Name + "." + componentType.PropertyNames[propertyIndex];
                var columnName = property.ColumnIterator.ElementAt(propertyIndex).Text;
                map.Add(propertyPath, columnName);
            }
        }

    }
}
