using Sistema.Reporter.Business.Interface;
using Microsoft.Extensions.Logging;

namespace Sistema.Reporter.Business.Service
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly RabbitMQConsumer _rabbitMQConsumer;
        private readonly ILogger<RabbitMQService> _logger;
        public RabbitMQService(
                RabbitMQConsumer rabbitMQConsumer
              , ILogger<RabbitMQService> logger)
        {
            _rabbitMQConsumer = rabbitMQConsumer;
            _logger = logger;
        }
        public async Task StartListening(string queueName, Action<string> messageProcessor)
        {
            _logger.LogInformation("Iniciando o consumo de mensagens do RabbitMQ.");
            await _rabbitMQConsumer.StartConsuming(queueName, messageProcessor);
        }
    }
}
