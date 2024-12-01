namespace Subscriber;

class Program
{
    static void Main(string[] args)
    {
        var subscriberService = new SubscriberService("host=rabbitmq;username=guest;password=guest");

        Console.WriteLine("Listening for RabbitMQ messages...");

        // Subscribe to the "AuditEvents" queue and print the message to the console.
        subscriberService.Subscribe<dynamic>("AuditEvents",
            message => { Console.WriteLine($"Received message: {message}"); }, CancellationToken.None);

        // Keep the console running until the user decides to stop.
        Console.ReadLine();
    }
}
