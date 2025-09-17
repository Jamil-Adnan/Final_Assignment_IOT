using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_segmentClient
{
    public class OrderItem
    {
        //public int OrderItemId { get; set; }

        //public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
    }
}
