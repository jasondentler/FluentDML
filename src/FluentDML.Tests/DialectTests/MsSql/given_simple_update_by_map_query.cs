﻿using NUnit.Framework;

namespace FluentDML.Tests.DialectTests.MsSql
{

    [TestFixture]
    public class given_simple_update_by_map_query : DialectTests.given_simple_update_by_map_query
    {
        public given_simple_update_by_map_query()
            : base(new MsSqlConfiguration())
        {
        }


    }
}
