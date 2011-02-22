using FluentDML.Dialect;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests.DialectTests.MsSql
{

    [TestFixture]
    public class constant_edge_cases : DialectTests.constant_edge_cases
    {

        protected override IDialect DB(Map map)
        {
            return new MsSqlDialect(map);
        }

    }
}
