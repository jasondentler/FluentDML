﻿using NUnit.Framework;

namespace FluentDML.Tests.DialectTests.MsSql
{

    [TestFixture]
    public class constant_edge_cases : DialectTests.constant_edge_cases
    {
        public constant_edge_cases() : base(new MsSqlConfiguration())
        {
        }
    }
}
