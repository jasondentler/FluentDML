using System;
using System.Collections.Generic;
using System.Reflection;

namespace FluentDML.Mapping
{

    public interface IMapMaker
    {
        Map MakeMap(Assembly assembly);
        Map MakeMap(Assembly assembly, Predicate<Type> filter);
        Map MakeMap(params Type[] types);
        Map MakeMap(IEnumerable<Type> types);

    }

}
