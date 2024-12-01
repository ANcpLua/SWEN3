using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Serialization.NewtonsoftJson;
using Microsoft.Extensions.Logging;

namespace Publisher;

public class PublisherService
{
    private readonly IBus _bus;
    private readonly ILogger<PublisherService>? _logger;
    private const int MaxRetries = 3;

    public PublisherService(string? connectionString, ILogger<PublisherService>? logger = null)
    {
        _logger = logger;
        _bus = RabbitHutch.CreateBus(connectionString,
            config => config.Register<ISerializer, NewtonsoftJsonSerializer>());
        // Log successful initialization if we have a logger
        _logger?.LogInformation("RabbitMQ connection initialized successfully");
    }

    public async Task PublishAsync<T>(T message)
    {
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                await _bus.PubSub.PublishAsync(message);
                _logger?.LogInformation("Published message of type {Type}", typeof(T).Name);
                return;
            }
            catch (Exception ex) when (attempt < MaxRetries)
            {
                var backoffSeconds = Math.Pow(2, attempt - 1);
                _logger?.LogWarning(ex, "Publish attempt {Attempt}/{Max} failed, retrying in {Delay}s",
                    attempt, MaxRetries, backoffSeconds);
                await Task.Delay(TimeSpan.FromSeconds(backoffSeconds));
            }
        }

        var finalException = new InvalidOperationException($"Failed to publish message after {MaxRetries} attempts");
        _logger?.LogError(finalException, "Publishing ultimately failed");
        throw finalException;
    }

    public Task PublishEventAsync(string id, string filePath, DateTime timestamp) =>
        PublishAsync(new { Id = id, FilePath = filePath, Timestamp = timestamp });
}