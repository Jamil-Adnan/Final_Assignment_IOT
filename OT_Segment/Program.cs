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
                .WithTcpServer("broker.mqtt.cool", 1883) // replace with "localhost" if running local broker
                .WithCleanSession()
                .Build();

            // When message arrives
            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    var order = JsonSerializer.Deserialize<OrderMessage>(json);

                    if (order == null)
                    {
                        Console.WriteLine("[OT Segment] Received invalid message.");
                        return;
                    }

                    Console.WriteLine($"[OT Segment] Received order {order.OrderId}, starting processing...");

                    // Simulate processing work
                    await Task.Delay(3000);

                    order.OrderStatus = "Processed";
                    order.OrderSent = true;

                    var processedMessage = new MqttApplicationMessageBuilder()
                        .WithTopic("orders/processed")
                        .WithPayload(JsonSerializer.Serialize(order))
                        .Build();

                    await mqttClient.PublishAsync(processedMessage);
                    Console.WriteLine($"[OT Segment] Order {order.OrderId} processed and order shipped.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[OT Segment] Error: {ex.Message}");
                }
            };

            // When connected
            mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("[OT Segment] Connected to MQTT broker.");
                await mqttClient.SubscribeAsync("orders/forward");
                //Console.WriteLine("[OT Segment] Subscribed to 'orders/forward'.");
            };

            // When disconnected
            mqttClient.DisconnectedAsync += e =>
            {
                Console.WriteLine("[OT Segment] Disconnected from MQTT broker.");
                return Task.CompletedTask;
            };

            Console.WriteLine("[OT Segment] connecting...");
            await mqttClient.ConnectAsync(options, CancellationToken.None);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            await mqttClient.DisconnectAsync();
        }
    }

    // Shared model (must match IT/Integration/Client)
    public class OrderMessage
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public bool Paid { get; set; }
        public string? OrderStatus { get; set; }
        public bool OrderSent { get; set; }
    }
}

