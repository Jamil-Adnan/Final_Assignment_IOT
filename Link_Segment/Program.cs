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
                Console.WriteLine($"[Integration App] Request Received: {message}");

                // forward as-is
                var msg = new MqttApplicationMessageBuilder()
                    .WithTopic("orders/forward")
                    .WithPayload(message)
                    .Build();

                await mqttClient.PublishAsync(msg);
                Console.WriteLine("[Integration App] Forwarded to OT");
            };

            mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("[Integration App] Connected to MQTT broker.");

                // Subscribe to paid orders
                await mqttClient.SubscribeAsync("orders/paid");
            };

            mqttClient.DisconnectedAsync += e =>
            {
                Console.WriteLine("[Integration App] Disconnected from MQTT broker.");
                return Task.CompletedTask;
            };

            Console.WriteLine("[Integration App] Connecting...");
            await mqttClient.ConnectAsync(options, CancellationToken.None);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            await mqttClient.DisconnectAsync();
        }
    }
}
