﻿using NUnit.Framework;

namespace FluentDML.Tests.DialectTests.MsSql
{

    [TestFixture]
    public class given_explicit_insert : DialectTests.given_explicit_insert
    {
        public given_explicit_insert()
            : base(new MsSqlConfiguration())
        {
        }


    }
}
