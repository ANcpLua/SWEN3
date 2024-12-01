using EasyNetQ;
using EasyNetQ.DI;
using EasyNetQ.Serialization.NewtonsoftJson;

namespace Subscriber;

public class SubscriberService
{
    private readonly IBus _bus;

    public SubscriberService(string? connectionString)
    {
        _bus = RabbitHutch.CreateBus(connectionString,
            config => { config.Register<ISerializer, NewtonsoftJsonSerializer>(); });
    }

    public void Subscribe<T>(string subscriptionId, Action<T> onMessage, CancellationToken cancellationToken)
    {
        Task.Run(() =>
        {
            try
            {
                _bus.PubSub.Subscribe(subscriptionId, onMessage, cancellationToken: cancellationToken);
                Console.WriteLine($"Subscribed to {subscriptionId}...");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"Subscription to {subscriptionId} was cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during subscription: {ex.Message}");
            }
        });
    }
}
