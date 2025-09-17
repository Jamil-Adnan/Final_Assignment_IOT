namespace IT_segmentClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient { BaseAddress = new Uri("https://localhost:5001/") };

            var customerClient = new CustomerClientApp(client);
            var productClient = new ProductClientApp(client);
            var orderClient = new OrderClientApp(client, customerClient);

            var menu = new MenuApp(productClient, customerClient, orderClient);
            await menu.RunAsync();
        }
    }
}
