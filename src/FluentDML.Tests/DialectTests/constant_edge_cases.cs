using System;
using System.Data;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests.DialectTests
{
    public abstract class constant_edge_cases : DialectTestFixture
    {
        
        protected override IDbConnection GetOpenConnection()
        {
            throw new NotImplementedException();
        }

        protected override IDbCommand GetCommand()
        {
            throw new NotImplementedException();
        }

        protected override Dialect.IDialect DB()
        {
            var mapMaker = new DefaultMapMaker();
            var map = mapMaker.MakeMap(typeof(Entity));
            return DB(map);
        }

        protected override void OnFixtureTearDown()
        {
        }
        
        [Test]
        public virtual void TruePredicate()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => true)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.Not.StringContaining("WHERE"));
        }

        [Test]
        public virtual void FalsePredicate()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => false)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("WHERE 1=0"));            
        }

        [Test]
        public virtual void TrueConstant()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => e.Flag1 == true)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("[Flag1] = 1"));            
        }


        [Test]
        public virtual void FalseConstant()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => e.Flag1 == false)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("[Flag1] = 0"));
        }

        [Test]
        public virtual void IsNull()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => e.Nullable == null)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("[Nullable] IS NULL"));
        }

        [Test]
        public virtual void IsNotNull()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => e.Nullable != null)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("[Nullable] IS NOT NULL"));
        }


        [Test]
        public virtual void BackwardIsNull()
        {
            var cmd = DB().Delete<Entity>()
                .Where(e => null == e.Nullable)
                .ToCommand();
            Assert.That(cmd.CommandText, Is.StringContaining("[Nullable] IS NULL"));
        }

        [Test]
        public virtual void BackwardIsNotNull()
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
