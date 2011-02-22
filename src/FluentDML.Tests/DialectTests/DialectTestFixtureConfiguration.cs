using System.Data;
using FluentDML.Dialect;
using FluentDML.Mapping;

namespace FluentDML.Tests.DialectTests
{
    public abstract class DialectTestFixtureConfiguration
    {

        public abstract IDialect GetDialect(Map map);

        public abstract IDbConnection GetOpenConnection();


    }
}
