using IT_SegmentApi.Models;
using MQTTnet;
using System.Text;
using System.Text.Json;

namespace Link_Segment
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var mqttClientFactory = new MqttClientFactory();
            var mqttClient = mqttClientFactory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.mqtt.cool", 1883)
                .WithCleanSession()
                .Build();

            // Handle incoming messages
            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var order = JsonSerializer.Deserialize<Order>(message);

                Console.WriteLine($"[Integration Stage] Received paid order {order.OrderId}");

                // Forward order to OT via MQTT
                var otMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("orders/forward")
                    .WithPayload(JsonSerializer.Serialize(order))
                    .Build();

                await mqttClient.PublishAsync(otMessage);
                Console.WriteLine($"[Integration Stage] Forwarded order {order.OrderId} to Process and Shipment.");
            };

            mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("Connected to MQTT broker.");

                // Subscribe to paid orders
                await mqttClient.SubscribeAsync("orders/paid");
            };

            mqttClient.DisconnectedAsync += e =>
            {
                Console.WriteLine("Disconnected from MQTT broker.");
                return Task.CompletedTask;
            };

            Console.WriteLine("Connecting...");
            await mqttClient.ConnectAsync(options, CancellationToken.None);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            await mqttClient.DisconnectAsync();
        }
    }
}
