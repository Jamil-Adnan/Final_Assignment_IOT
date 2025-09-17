namespace IT_SegmentApi.DTOs
{
    public class OrderDto
    {
        public class CreateOrderDto
        {
            public int CustomerId { get; set; }
            public List<CreateOrderItemDto> Items { get; set; } = new();
        }

        public class CreateOrderItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
