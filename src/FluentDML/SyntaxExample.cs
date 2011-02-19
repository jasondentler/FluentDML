using System;
using FluentDML.Dialect;

namespace FluentDML
{
    internal class SyntaxExample
    {

        public SyntaxExample()
        {
            var db = GetDialect();
            var dto = new SomeDTO();

            db.Insert<SomeEntity>()
                .MapFrom(dto);

            db.Update<SomeEntity>()
                .Set(c => c.Value1, dto.Value1)
                .Set(c => c.Value2, dto.Value2)
                .Where(c => c.Id == dto.Id)
                .And(c => c.Value2 == 1)
                .ToCommand();

            db.Update<SomeEntity>()
                .MapFrom(dto)
                .WithId(c => c.Id);
            
            db.Delete<SomeEntity>()
                .Where(c => c.Id == dto.Id)
                .And(c => c.Value2 == dto.Value2)
                .ToCommand();

        }


        protected class SomeEntity
        {
            public Guid Id { get; set; }
            public string Value1 { get; set; }
            public int Value2 { get; set; }
        }

        protected class SomeDTO
        {
            public Guid Id { get; set; }
            public string Value1 { get; set; }
            public int Value2 { get; set; }
        }

        private IDialect GetDialect()
        {
            throw new NotImplementedException();
        }

    }
}
