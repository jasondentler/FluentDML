using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentDML.Dialect;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests
{
    [TestFixture]
    public class constant_edge_cases : BaseFixture
    {

        private IDialect DB()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof (Entity));
            return new MsSqlDialect(map);
        }

        [Test]
        public void TruePredicate()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => true)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.Not.StringContaining("WHERE"));
        }

        [Test]
        public void FalsePredicate()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => false)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("WHERE 1=0"));
        }

        [Test]
        public void TrueConstant()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => e.Flag1 == true)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("[Flag1] = 1"));
        }

        [Test]
        public void FalseConstant()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => e.Flag1 == false)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("[Flag1] = 0"));
        }

        [Test]
        public void IsNull()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => e.Nullable == null)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("[Nullable] IS NULL"));
        }

        [Test]
        public void IsNotNull()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => e.Nullable != null)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("[Nullable] IS NOT NULL"));
        }


        [Test]
        public void BackwardIsNull()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => null == e.Nullable)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("[Nullable] IS NULL"));
        }

        [Test]
        public void BackwardIsNotNull()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => null != e.Nullable)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("[Nullable] IS NOT NULL"));
        }

        public class Entity
        {
            public string Nullable { get; set; }
            public bool Flag1 { get; set; }
            public bool Flag2 { get; set; }
            public bool Flag3 { get; set; }
            public bool Flag4 { get; set; }
        }

    }
}
