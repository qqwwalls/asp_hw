using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace RabbitMqReader;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        string hostName = configuration["RabbitMqSettings:HostName"] ?? "localhost";
        int port = int.Parse(configuration["RabbitMqSettings:Port"] ?? "5672");

        var factory = new ConnectionFactory()
        {
            HostName = hostName,
            Port = port
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "Users",
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        Console.WriteLine(" [*] Waiting for user registrations. To exit press CTRL+C");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] Received new user data: {message}");

            // Acknowledge the message so it gets removed from the queue
            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(queue: "Users",
                                        autoAck: false, // Important to test manual ack
                                        consumer: consumer);

        Console.ReadLine();
    }
}
