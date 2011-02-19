using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentDML.Tests
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; }
        public Address Billing { get; set; }
    }
}
