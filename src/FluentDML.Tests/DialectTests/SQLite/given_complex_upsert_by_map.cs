using NUnit.Framework;

namespace FluentDML.Tests.DialectTests.SQLite
{

    [TestFixture]
    public class given_complex_upsert_by_map : DialectTests.given_complex_upsert_by_map
    {
        public given_complex_upsert_by_map() : base(new MsSql.MsSqlConfiguration())
        {
        }
    }

}
