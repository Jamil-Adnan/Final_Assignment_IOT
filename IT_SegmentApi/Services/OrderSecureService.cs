using IT_SegmentApi.Data;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using System.Text.Json;

namespace IT_SegmentApi.Services
{
    public class OrderSecureService : BackgroundService
    {
        private readonly IServiceProvider _sp;
        public OrderSecureService(IServiceProvider sp)
        {
            _sp = sp;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<IOTFinalDbContext>();

                        // Find orders that are Paid but not Sent
                      var unsentOrders = await db.Orders
                            .Where(o => o.Paid && !o.OrderSent)
                            .Include(o => o.Items)
                            .ToListAsync(stoppingToken);

                    if (unsentOrders.Any())
                    {
                        Console.WriteLine($"[ResendService] Found {unsentOrders.Count} unshipped orders. Republishing...");

                        var factory = new MqttClientFactory();
                        var client = factory.CreateMqttClient();
                        var options = new MqttClientOptionsBuilder().WithTcpServer("broker.mqtt.cool", 1883).WithCleanSession().Build();

                        await client.ConnectAsync(options, stoppingToken);

                        foreach (var order in unsentOrders)
                        {
                            var payload = JsonSerializer.Serialize(new
                            {
                                order.OrderId,
                                order.CustomerId,
                                order.Paid,
                                order.OrderStatus,
                                order.OrderSent,
                                Items = order.Items.Select(i => new { i.ProductId, i.Quantity, i.ItemPrice })
                            });

                            var msg = new MqttApplicationMessageBuilder()
                                    .WithTopic("orders/paid")
                                    .WithPayload(payload)
                                    .Build();

                            await client.PublishAsync(msg, stoppingToken);

                            Console.WriteLine($"[ResendService] Republished order {order.OrderId}");
                        }

                        await client.DisconnectAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ResendService] Error: {ex.Message}");
                }

                    // Sleep for 6 hours
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }
    }
}
