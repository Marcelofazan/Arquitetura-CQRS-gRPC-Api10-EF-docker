namespace InfraEstrutura.Producao.Business
{
    public interface IRabbitMQPublisher
    {
        Task PublishMessage(string queueName, string message);
    }
}
