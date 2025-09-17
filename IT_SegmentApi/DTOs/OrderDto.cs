namespace IT_SegmentApi.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Paid { get; set; }
        public string OrderStatus { get; set; }
        public bool OrderSent { get; set; }      // <- NEW for tracking OT completion
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
