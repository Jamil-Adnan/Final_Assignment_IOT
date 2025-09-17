using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace IT_segmentClient
{
    public class OrderClientApp
    {
        private readonly HttpClient _client;
        private readonly CustomerClientApp _customerClient;

        public OrderClientApp(HttpClient client, CustomerClientApp customerClient)
        {
            _client = client;
            _customerClient = customerClient;
        }

        public async Task PlaceOrder()
        {
            var customer = _customerClient.GetLoggedIn();
            if (customer == null)
            {
                Console.WriteLine("Please login first.");
                return;
            }

            var dto = new
            {
                CustomerId = customer.CustomerId,
                Items = new List<object>()
            };

            while (true)
            {
                Console.Write("Enter Product Id (0 to finish): ");
                if (!int.TryParse(Console.ReadLine(), out int id) || id == 0) break;

                Console.Write("Quantity: ");
                if (!int.TryParse(Console.ReadLine(), out int qty)) continue;

                dto.Items.Add(new { ProductId = id, Quantity = qty });
            }

            if (!dto.Items.Any())
            {
                Console.WriteLine("No items added. Order cancelled.");
                return;
            }

            var response = await _client.PostAsJsonAsync("api/orders", dto);
            if (response.IsSuccessStatusCode)
            {
                var order = await response.Content.ReadFromJsonAsync<Order>();
                Console.WriteLine($"Order with Order ID {order.OrderId} created. Please pay {order.TotalAmount:C}.");
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to place order: {err}");
            }
        }

        public async Task ViewMyOrders()
        {
            var customer = _customerClient.GetLoggedIn();
            if (customer == null)
            {
                Console.WriteLine("Please login first.");
                return;
            }

            var orders = await _client.GetFromJsonAsync<List<Order>>($"api/orders/customer/{customer.CustomerId}");
            if (orders == null || orders.Count == 0)
            {
                Console.WriteLine("No orders found.");
                return;
            }

            Console.WriteLine("\n=== My Orders ===");
            foreach (var o in orders)
            {
                Console.WriteLine($"Order ID {o.OrderId} Created on {o.OrderDate:d}, Total amount {o.TotalAmount:C}, Order status {o.OrderStatus} | Paid: {o.Paid}");
                foreach (var item in o.Items)
                {
                    Console.WriteLine($"   -> Product {item.ProductId} x{item.Quantity} @ {item.ItemPrice:C}");
                }
            }
        }

        public async Task PayOrder()
        {
            var customer = _customerClient.GetLoggedIn();
            if (customer == null)
            {
                Console.WriteLine("Please login first.");
                return;
            }

            Console.Write("Enter Order Id to pay: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId)) return;

            Console.Write("Pay now? (y/n): ");
            bool pay = Console.ReadLine()?.Trim().ToLower() == "y";

            var response = await _client.PutAsJsonAsync($"api/orders/{orderId}/pay", pay);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Order>();
                Console.WriteLine($"Order Id {result.OrderId} updated. Paid: {result.Paid}, Status: {result.OrderStatus}. Awaiting Shipment.");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to update payment: {error}");
            }
        }
    }
}
