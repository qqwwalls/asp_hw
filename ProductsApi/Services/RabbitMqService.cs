using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ProductsApi.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly IConfiguration _configuration;
    private readonly string _hostname;
    private readonly int _port;

    public RabbitMqService(IConfiguration configuration)
    {
        _configuration = configuration;
        _hostname = _configuration["RabbitMqSettings:HostName"] ?? "localhost";
        _port = int.Parse(_configuration["RabbitMqSettings:Port"] ?? "5672");
    }

    public async Task SendMessageAsync<T>(T message, string queueName)
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            Port = _port
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queueName,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(exchange: "",
                                        routingKey: queueName,
                                        mandatory: false,
                                        basicProperties: new RabbitMQ.Client.BasicProperties(),
                                        body: body);
    }
}
