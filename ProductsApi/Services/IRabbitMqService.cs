using System.Threading.Tasks;

namespace ProductsApi.Services;

public interface IRabbitMqService
{
    Task SendMessageAsync<T>(T message, string queueName);
}
