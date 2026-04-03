using System;
using System.Collections.Generic;
using System.Text;

namespace TaxiSubscriber.Model
{
    internal class Order
    {
        public string Id { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public bool QuickOrder { get; set; }
        public DateTime? PickUpTime { get; set; }
    }
}
