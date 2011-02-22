using NUnit.Framework;

namespace FluentDML.Tests.DialectTests.SQLite
{

    [TestFixture]
    public class given_explicit_update_query : DialectTests.given_explicit_update_query
    {
        public given_explicit_update_query()
            : base(new SQLiteConfiguration())
        {
        }


    }
}
