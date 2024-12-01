using Microsoft.Extensions.Logging;
using Publisher;

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole().SetMinimumLevel(LogLevel.Information); // Adjust log level as needed
});

var logger = loggerFactory.CreateLogger<PublisherService>();
var connectionString = "host=rabbitmq;username=guest;password=guest";

var publisherService = new PublisherService(connectionString, logger);

var message = new { Event = "DocumentUploaded", DocumentId = 123 };

// Attempt to publish the message with retries
try
{
    await publisherService.PublishAsync(message);
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to publish message after retries.");
}