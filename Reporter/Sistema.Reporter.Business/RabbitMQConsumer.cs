using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMQConsumer : IAsyncDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMQConsumer> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    // Semáforo para garantir que apenas uma conexão seja criada por vez de forma segura
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public RabbitMQConsumer(IConnectionFactory connectionFactory, ILogger<RabbitMQConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    private async Task EnsureInitializedAsync()
    {
        if (_channel != null) return;

        await _connectionLock.WaitAsync();
        try
        {
            if (_channel == null)
            {
                _connection = await _connectionFactory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();
                _logger.LogInformation("RabbitMQ connection and channel established.");
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task StartConsuming(string queueName, Action<string> onMessageReceived)
    {
        // Garante que o canal está pronto antes de continuar
        await EnsureInitializedAsync();

        await _channel!.QueueDeclareAsync(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received message: {message}", message);

                onMessageReceived(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing received message.");
            }

            await Task.CompletedTask;
        };

        // Iniciar o consumo e manter o canal ativo
        await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        _logger.LogInformation("Consuming messages from queue: {queueName}", queueName);
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Closing RabbitMQ connection and channel...");

        // No RabbitMQ.Client moderno, basta descartar o canal e a conexão assincronamente
        if (_channel != null)
        {
            await _channel.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }

        _connectionLock.Dispose();
        GC.SuppressFinalize(this);
    }
}