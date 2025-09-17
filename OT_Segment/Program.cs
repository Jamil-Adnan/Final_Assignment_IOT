using MQTTnet;
using System.Text;
using System.Text.Json;

namespace OT_Segment
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var mqttClientFactory = new MqttClientFactory();
            var mqttClient = mqttClientFactory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.mqtt.cool", 1883) // or localhost if using a local broker
                .WithCleanSession()
                .Build();

            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Console.WriteLine($"[OT Segment] Received order: {message}");

                var order = JsonSerializer.Deserialize<Order>(message);

                // Simulate processing delay
                Console.WriteLine($"[OT Segment] Processing order {order.OrderId}...");
                await Task.Delay(3000);

                // Update order status
                order.OrderStatus = "Processed";

                var processedMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("orders/processed")
                    .WithPayload(JsonSerializer.Serialize(order))
                    .Build();

                await mqttClient.PublishAsync(processedMessage);
                Console.WriteLine($"[OT Segment] Order Id {order.OrderId} processed and Shipped.");
            };

            mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("OT connected to MQTT broker.");
                await mqttClient.SubscribeAsync("orders/forward");
                //Console.WriteLine("Subscribed to 'orders/forward'.");
            };

            mqttClient.DisconnectedAsync += e =>
            {
                Console.WriteLine("OT disconnected from MQTT broker.");
                return Task.CompletedTask;
            };

            Console.WriteLine("OT App connecting...");
            await mqttClient.ConnectAsync(options, CancellationToken.None);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            await mqttClient.DisconnectAsync();
        }
    }

    // Shared model (should match IT/Integration)
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public bool Paid { get; set; }
        public string OrderStatus { get; set; }
    }
}

