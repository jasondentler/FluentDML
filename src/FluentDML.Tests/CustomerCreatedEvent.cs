using System;

namespace FluentDML.Tests
{
    public class CustomerCreatedEvent
    {
        public Guid EventSourceId { get; set; }
        public string Name { get; set; }
    }
}