using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProductsApi.Data;
using ProductsApi.DTOs;
using ProductsApi.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ProductsApi.Services;

public class OrderProcessingService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public OrderProcessingService(IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string hostName = _configuration["RabbitMqSettings:HostName"] ?? "localhost";
        int port = int.Parse(_configuration["RabbitMqSettings:Port"] ?? "5672");

        var factory = new ConnectionFactory()
        {
            HostName = hostName,
            Port = port
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue: "Orders",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageString = Encoding.UTF8.GetString(body);
            
            try
            {
                var orderMessage = JsonSerializer.Deserialize<OrderMessage>(messageString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (orderMessage != null)
                {
                    await ProcessOrderAsync(orderMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing order: {ex.Message}");
            }
            finally
            {
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
        };

        await _channel.BasicConsumeAsync(queue: "Orders",
                                        autoAck: false,
                                        consumer: consumer);
                                        
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessOrderAsync(OrderMessage orderMessage)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        bool hasEnoughStock = true;
        decimal totalPrice = 0;
        var emailBodyBuilder = new StringBuilder();
        
        emailBodyBuilder.AppendLine("<h3>Order Details</h3>");
        emailBodyBuilder.AppendLine("<ul>");

        var order = new Order
        {
            UserId = orderMessage.UserId,
            Status = "Processing",
            Paid = false
        };

        foreach (var item in orderMessage.Items)
        {
            var product = await context.Products.FindAsync(item.ProductId);
            if (product == null || product.StockCount < item.Count)
            {
                hasEnoughStock = false;
                break;
            }

            var itemTotal = product.Price * item.Count;
            totalPrice += itemTotal;
            emailBodyBuilder.AppendLine($"<li>{product.Name} - {item.Count}x @ {product.Price} = {itemTotal}</li>");

            product.StockCount -= item.Count;

            order.OrderDetails.Add(new OrderDetail
            {
                ProductId = product.Id,
                Price = product.Price,
                Count = item.Count
            });
        }

        if (hasEnoughStock)
        {
            emailBodyBuilder.AppendLine("</ul>");
            emailBodyBuilder.AppendLine($"<p><strong>Total Price: {totalPrice}</strong></p>");
            
            order.Status = "Approved";
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            string emailContent = $"<h2>Your order has been approved!</h2>{emailBodyBuilder.ToString()}";
            await emailService.SendEmailAsync(orderMessage.UserEmail, "Order Approved", emailContent);
            Console.WriteLine($"Order {order.Id} approved and email sent to {orderMessage.UserEmail}.");
        }
        else
        {
            string emailContent = "<h2>Order Waiting</h2><p>Sorry, some of the products in your order are currently out of stock. We will process your order as soon as they become available.</p>";
            await emailService.SendEmailAsync(orderMessage.UserEmail, "Order Waiting on Stock", emailContent);
            Console.WriteLine($"Order from {orderMessage.UserEmail} failed due to lack of stock. Email sent.");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync();
        if (_connection != null) await _connection.CloseAsync();
        await base.StopAsync(cancellationToken);
    }
}
