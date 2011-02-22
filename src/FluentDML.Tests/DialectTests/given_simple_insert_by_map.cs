﻿using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using AutoMapper;
using FluentDML.Dialect;
using FluentDML.Mapping;
using NUnit.Framework;

namespace FluentDML.Tests.DialectTests
{
    public abstract class given_simple_insert_by_map : DialectTestFixture
    {

        protected override void OnFixtureSetup()
        {
            base.OnFixtureSetup();
            Mapper.Reset();
            Mapper.CreateMap<Address, Address>();
            Mapper.CreateMap<CustomerCreatedEvent, Customer>()
                .ForMember(c => c.CustomerId, mo => mo.MapFrom(e => e.EventSourceId))
                .ForMember(c => c.Billing, mo => mo.Ignore());
            Mapper.CreateMap<CustomerMovedEvent, Customer>()
                .ForMember(c => c.CustomerId, mo => mo.MapFrom(e => e.EventSourceId))
                .ForMember(c => c.Name, mo => mo.Ignore())
                .ForMember(c => c.Billing, mo => mo.MapFrom(e => new Address() { City = e.BillingCity }));
            Mapper.AssertConfigurationIsValid();
        }

        protected  override IDbCommand GetCommand()
        {
            return DB().Insert<Customer>()
                .MapFrom(_event);
        }

        private CustomerCreatedEvent _event = new CustomerCreatedEvent() { EventSourceId = Guid.NewGuid(), Name = "Jason" };


        [Test]
        public virtual void it_generates_insert_sql()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringStarting("INSERT INTO [Customer] ("));
        }

        [Test]
        public virtual void it_generates_column_list()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("[CustomerId],[Name]"));
        }

        [Test]
        public virtual void it_generates_parameter_list()
        {
            var cmd = GetCommand();
            var sql = cmd.CommandText;
            Assert.That(sql, Is.StringContaining("@p0,@p1"));
        }

        [Test]
        public virtual void it_generates_where_parameters()
        {
            var cmd = GetCommand();
            Assert.That(GetParam(cmd, "p0").Value, Is.EqualTo(_event.EventSourceId));
            Assert.That(GetParam(cmd, "p1").Value, Is.EqualTo(_event.Name));
        }

        [Test]
        public virtual void it_inserted_a_row()
        {
            int rows;
            var insert = GetCommand();

            using (var conn = GetOpenConnection())
            {
                insert.Connection = conn;
                rows = insert.ExecuteNonQuery();
                conn.Close();
            }
            Assert.That(rows, Is.EqualTo(1));
        }


    }
}