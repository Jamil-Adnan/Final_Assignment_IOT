namespace IT_SegmentApi.DTOs
{
    public class OrderItemDto
    {
        public int ProductId { get; set; }        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }   // set by server from Product.Price
    }
}
