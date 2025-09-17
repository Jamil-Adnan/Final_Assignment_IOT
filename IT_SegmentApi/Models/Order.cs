namespace IT_SegmentApi.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public bool Paid { get; set; } = false;
        public string OrderStatus { get; set; } = "Pending";
        public bool Shipment { get; set; } = false;
        public Customer Customer { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
