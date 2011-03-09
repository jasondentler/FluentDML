using System;

namespace FluentDML.Tests
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public Address Billing { get; set; }
        public DayOfWeek DayOfTheWeek { get; set; }

    }
}
