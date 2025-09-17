using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace IT_segmentClient
{
    public class CustomerClientApp
    {
        private readonly HttpClient _client;
        private Customer? _loggedIn;

        public CustomerClientApp(HttpClient client)
        {
            _client = client;
        }

        public async Task Register()
        {
            Console.Write("First Name: ");
            string first = Console.ReadLine();
            Console.Write("Last Name: ");
            string last = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Phone: ");
            string phone = Console.ReadLine();
            Console.Write("Address: ");
            string address = Console.ReadLine();
            Console.Write("Postcode: ");
            string postcode = Console.ReadLine();
            Console.Write("City: ");
            string city = Console.ReadLine();
            Console.Write("Country: ");
            string country = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            var dto = new
            {
                FirstName = first,
                LastName = last,
                Email = email,
                Phone = phone,
                Address = address,
                Postcode = postcode,
                City = city,
                Country = country,
                HashPassword = password
            };

            var response = await _client.PostAsJsonAsync("api/customers/register", dto);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("User registered successfully!");
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"User registration failed: {err}");
            }
        }

        public async Task Login()
        {
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Password: ");
            string password = Console.ReadLine();

            var dto = new { Email = email, HashPassword = password };
            var response = await _client.PostAsJsonAsync("api/customers/login", dto);

            if (response.IsSuccessStatusCode)
            {
                _loggedIn = await response.Content.ReadFromJsonAsync<Customer>();
                Console.WriteLine($"Logged in as {_loggedIn.FirstName} {_loggedIn.LastName}");
            }
            else
            {
                Console.WriteLine("Wrong Email or Password.");
            }
        }

        public Customer? GetLoggedIn() => _loggedIn;
        public void Logout() => _loggedIn = null;
    }
}
