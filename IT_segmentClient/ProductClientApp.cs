using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace IT_segmentClient
{
    public class ProductClientApp
    {
        private readonly HttpClient _client;

        public ProductClientApp(HttpClient client)
        {
            _client = client;
        }

        public async Task ShowProducts()
        {
            var products = await _client.GetFromJsonAsync<List<Product>>("api/products");
            Console.WriteLine("\n=== Products ===");
            foreach (var product in products)
            {
                Console.WriteLine($"Product Id: {product.ProductId}, Name: {product.Name}, Description: {product.Description}, Unit price: {product.Price:C}, Available stock: {product.Stock}");
            }
        }
    }
}
