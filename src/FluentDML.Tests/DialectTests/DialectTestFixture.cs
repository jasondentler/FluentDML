using System.Data;
using FluentDML.Dialect;
using FluentDML.Mapping;

namespace FluentDML.Tests.DialectTests
{
    public abstract class DialectTestFixture : BaseFixture
    {

        protected virtual IDialect DB()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Customer));
            return DB(map);
        }

        protected abstract IDialect DB(Map map);

        protected abstract IDbConnection GetOpenConnection();

        protected abstract IDbCommand GetCommand();

        protected virtual IDbDataParameter GetParam(IDbCommand command, string name)
        {
            return (IDbDataParameter)command.Parameters[name];
        }

        protected override void OnFixtureTearDown()
        {
            using (var conn = GetOpenConnection())
            {
                var cmd = DB().Delete<Customer>()
                    .Where(c => true)
                    .ToCommand();
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            base.OnFixtureTearDown();
        }


    }
}
