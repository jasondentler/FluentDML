using NUnit.Framework;

namespace FluentDML.Tests.DialectTests.SQLite
{

    [TestFixture]
    public class given_delete_with_complex_where : DialectTests.given_delete_with_complex_where
    {
        public given_delete_with_complex_where()
            : base(new SQLiteConfiguration())
        {
        }


    }
}
