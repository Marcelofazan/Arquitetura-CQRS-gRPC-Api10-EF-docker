using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfraEstrutura.Producao.Business
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly IConnectionFactory _connectionFactory;

        public RabbitMQPublisher(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public RabbitMQPublisher() { }

        public virtual async Task PublishMessage(string queueName, string message)
        {

            var factory = new ConnectionFactory { HostName = "localhost" };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // 2. Declaração da fila assíncrona
            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var body = Encoding.UTF8.GetBytes(message);

            // 3. Envio da mensagem assíncrono (usando as novas propriedades)
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                mandatory: false,
                basicProperties: new BasicProperties(), // OBRIGATÓRIO: Não aceita mais null nas versões novas
                body: body
            );
        }
    }
}
