using System;

namespace FluentDML
{
    class SyntaxExample
    {

        public SyntaxExample()
        {
            Update<SyntaxExample>()
                .Set(c => c.Value1, "name")
                .Set(c => c.Value2, 1)
                .Where(c => c.Id == new Guid())
                .And(c => c.Value2 == 1);

            Update<SyntaxExample>()
                .MapFrom(new object())
                .WithId(c => c.Id);

            Upsert<SyntaxExample>()
                .Set(c => c.Value1, "name")
                .Set(c => c.Value2, 1)
                .WithId(c => c.Id);

            Upsert<SyntaxExample>()
                .MapFrom(new object())
                .WithId(c => c.Id);

            Delete<SyntaxExample>()
                .Where(c => c.Id == new Guid());
        }

        public IDelete<T> Delete<T>()
        {
            throw new NotImplementedException();
        }

        public IUpsert<T> Upsert<T>()
        {
            throw new NotImplementedException();
        }

        public IUpdate<T> Update<T>()
        {
            throw new NotImplementedException();
        }

        public string Value1 { get; set; }
        public int Value2 { get; set; }

        public Guid Id { get; set; }

    }
}
