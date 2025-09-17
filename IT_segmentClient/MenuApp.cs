using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_segmentClient
{
    public class MenuApp
    {
        private readonly ProductClientApp _productClient;
        private readonly CustomerClientApp _customerClient;
        private readonly OrderClientApp _orderClient;

        public MenuApp(ProductClientApp productClient, CustomerClientApp customerClient, OrderClientApp orderClient)
        {
            _productClient = productClient;
            _customerClient = customerClient;
            _orderClient = orderClient;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\n=== MENU ===");
                Console.WriteLine("1. Show Products");
                Console.WriteLine("2. Register");
                Console.WriteLine("3. Login");
                Console.WriteLine("4. Logout");
                Console.WriteLine("5. Place Order");
                Console.WriteLine("6. View My Orders");
                Console.WriteLine("7. Pay for Order");
                Console.WriteLine("8. Exit");
                Console.Write("Chose a menu: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await _productClient.ShowProducts();
                        break;
                    case "2":
                        await _customerClient.Register();
                        break;
                    case "3":
                        await _customerClient.Login();
                        break;
                    case "4":
                        _customerClient.Logout();
                        Console.WriteLine("Logged out.");
                        break;
                    case "5":
                        await _orderClient.PlaceOrder();
                        break;
                    case "6":
                        await _orderClient.ViewMyOrders();
                        break;
                    case "7":
                        await _orderClient.PayOrder();
                        break;
                    case "8":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
            }
        }
    }
}
