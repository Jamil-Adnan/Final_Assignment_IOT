using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_segmentClient
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Paid { get; set; }
        public string OrderStatus { get; set; }
        //public bool Shipment { get; set; } = false;
        public List<OrderItem> Items { get; set; } = new();
    }
}
