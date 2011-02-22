using NUnit.Framework;

namespace FluentDML.Tests.DialectTests.SQLite
{

    [TestFixture]
    public class given_simple_insert_by_map : DialectTests.given_simple_insert_by_map
    {
        public given_simple_insert_by_map()
            : base(new MsSql.MsSqlConfiguration())
        {
        }


    }
}
