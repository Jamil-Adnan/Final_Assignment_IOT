using IT_SegmentApi.Data;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using System.Text;
using System.Text.Json;

namespace IT_SegmentApi.Services
{
    public class MqttProcessedListener : BackgroundService
    {
        private readonly IServiceProvider _sp;

        public MqttProcessedListener(IServiceProvider sp)
        {
            _sp = sp;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var mqttClientFactory = new MqttClientFactory();
            var mqttClient = mqttClientFactory.CreateMqttClient();
            /*var factory = new MqttClientFactory();
            var client = factory.CreateMqttClient();*/
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.mqtt.cool", 1883) // or "localhost"
                .WithCleanSession()
                .Build();

            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var json = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                try
                {
                    var orderMsg = JsonSerializer.Deserialize<OrderMessage>(json);
                    if (orderMsg != null)
                    {
                        using var scope = _sp.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<IOTFinalDbContext>();

                        var dbOrder = await db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderMsg.OrderId, stoppingToken);
                        if (dbOrder != null)
                        {
                            dbOrder.OrderStatus = "Processed";
                            dbOrder.OrderSent = true;  // ✅ mark sent
                            await db.SaveChangesAsync(stoppingToken);

                            Console.WriteLine($"[API] Order {dbOrder.OrderId} updated → Sent={dbOrder.OrderSent}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[API] Error processing message: {ex.Message}");
                }
            };

            mqttClient.ConnectedAsync += async e =>
            {
                Console.WriteLine("[API] Connected to MQTT broker.");
                await mqttClient.SubscribeAsync("orders/processed");
                Console.WriteLine("[API] Subscribed to 'orders/processed'.");
            };

            await mqttClient.ConnectAsync(options, stoppingToken);
        }

        // Minimal message type that matches OT payload
        private class OrderMessage
        {
            public int OrderId { get; set; }
            public int CustomerId { get; set; }
            public bool Paid { get; set; }
            public string? OrderStatus { get; set; }
            public bool OrderSent { get; set; }
        }
    }
}
