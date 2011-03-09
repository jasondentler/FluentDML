using NUnit.Framework;

namespace FluentDML.Tests.DialectTests.MsSql
{

    [TestFixture]
    public class given_delete_with_constant_enum_predicate : DialectTests.given_delete_with_constant_enum_predicate
    {
        public given_delete_with_constant_enum_predicate()
            : base(new MsSqlConfiguration())
        {
        }


    }
}
