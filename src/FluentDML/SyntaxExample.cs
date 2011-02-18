using System;

namespace FluentDML
{
    internal class SyntaxExample
    {

        public SyntaxExample()
        {

            var evnt = new SomeEvent();

            Update<SomeViewModel>()
                .Set(c => c.Value1, "name")
                .Set(c => c.Value2, 1)
                .Where(c => c.Id == new Guid())
                .And(c => c.Value2 == 1)
                .ToCommand();

            Update<SomeViewModel>()
                .MapFrom(evnt)
                .WithId(c => c.Id);

            Upsert<SomeViewModel>()
                .Set(c => c.Value1, "name")
                .Set(c => c.Value2, 1)
                .WithId(c => c.Id);

            Upsert<SomeViewModel>()
                .MapFrom(evnt)
                .WithId(c => c.Id);

            Delete<SomeViewModel>()
                .Where(c => c.Id == new Guid())
                .And(c => c.Value2 == 1)
                .ToCommand();
                
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

        protected class SomeViewModel
        {
            public Guid Id { get; set; }
            public string Value1 { get; set; }
            public int Value2 { get; set; }
        }

        protected class SomeEvent
        {
            public Guid Id { get; set; }
            public string Value1 { get; set; }
            public int Value2 { get; set; }
        }


    }
}
