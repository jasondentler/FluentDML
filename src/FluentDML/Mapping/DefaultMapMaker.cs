using System;
using System.Reflection;

namespace FluentDML.Mapping
{
    public class DefaultMapMaker : MapMaker
    {
        protected override ClassMap MakeClassMap(Type type)
        {
            var classMap = new ClassMap(type, type.Name);
            MapProperties(classMap, type, "");
            return classMap;
        }

        protected virtual void MapProperties(ClassMap classMap, Type type, string propertyPath)
        {
            foreach (var property in type.GetProperties())
                MapProperty(classMap, property, propertyPath);
        }

        protected virtual void MapProperty(ClassMap classMap, PropertyInfo property, string propertyPath)
        {
            var newPath = AddToPath(propertyPath, property.Name);
            if (TypeIsComponent(property.PropertyType))
            {
                MapProperties(classMap, property.PropertyType, newPath);
            } else
            {
                classMap.Add(newPath, newPath.Replace(".", "_"));
            }
        }

        protected virtual string AddToPath(string propertyPath, string propertyToAdd)
        {
            if (string.IsNullOrEmpty(propertyPath))
                return propertyToAdd;
            return string.Format("{0}.{1}", propertyPath, propertyToAdd);
        }

        protected virtual bool TypeIsComponent(Type type)
        {
            return !type.Namespace.StartsWith("System");
        }

    }
}
