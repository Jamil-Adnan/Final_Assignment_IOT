using IT_SegmentApi.Data;
using IT_SegmentApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using System.Text.Json;
using static IT_SegmentApi.DTOs.OrderDto;

namespace IT_SegmentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOTFinalDbContext _context;

        public OrdersController(IOTFinalDbContext context)
        {
            _context = context;
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CreateOrderDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest("Order must contain at least one item.");

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                Paid = false,
                Shipment = false,
                Items = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"Product {item.ProductId} not found.");

                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ItemPrice = product.Price
                });

                total += item.Quantity * product.Price;

                // Optional: decrease stock
                product.Stock -= item.Quantity;
            }

            order.TotalAmount = total;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                order.OrderId,
                order.CustomerId,
                order.TotalAmount,
                order.OrderStatus,
                order.Paid,
                Items = order.Items.Select(i => new { i.ProductId, i.Quantity, i.ItemPrice })
            });
        }

        // GET: api/orders/customer/5
        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetCustomerOrders(int id)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == id)
                .Include(o => o.Items)
                .ToListAsync();

            return Ok(orders.Select(o => new
            {
                o.OrderId,
                o.CustomerId,
                o.OrderDate,
                o.TotalAmount,
                o.OrderStatus,
                o.Paid,
                Items = o.Items.Select(i => new { i.ProductId, i.Quantity, i.ItemPrice })
            }));
        }

        // PUT: api/orders/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.OrderStatus = status;
            await _context.SaveChangesAsync();

            return Ok(new { order.OrderId, order.OrderStatus });
        }


        [HttpPut("{id}/pay")]
        public async Task<IActionResult> PayOrder(int id, [FromBody] bool paid)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound("Order not found.");

            if (order.Paid)
            {
                return BadRequest(new { message = $"Order {id} is already paid." });
            }

            if (paid)
            {
                order.Paid = true;
                order.OrderStatus = "Paid";

                // Optional: trigger integration (send to OT or mark in DB)
                await _context.SaveChangesAsync();

                var mqttClientFactory = new MqttClientFactory();
                var mqttClient = mqttClientFactory.CreateMqttClient();

                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer("broker.mqtt.cool", 1883)
                    .Build();

                await mqttClient.ConnectAsync(options);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("orders/paid")
                    .WithPayload(JsonSerializer.Serialize(order))
                    .Build();

                await mqttClient.PublishAsync(message);
                await mqttClient.DisconnectAsync();
                return Ok(order);
            }

            return BadRequest(new { message = "Invalid request. Payment must be true." });
        }
    }
}
