using System;

namespace FluentDML.Tests
{
    public class CustomerMovedEvent
    {
        public Guid EventSourceId { get; set; }
        public string BillingCity { get; set; }
    }
}