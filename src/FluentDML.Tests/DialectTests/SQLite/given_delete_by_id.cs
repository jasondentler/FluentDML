using NUnit.Framework;

namespace FluentDML.Tests.DialectTests.SQLite
{

    [TestFixture]
    public class given_delete_by_id : DialectTests.given_delete_by_id
    {
        public given_delete_by_id()
            : base(new SQLiteConfiguration())
        {
        }


    }
}
