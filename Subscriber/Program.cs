using EasyNetQ;
using Messages;

namespace Subscriber;

public class Program
{
    static async Task Main(string[] args)
    {
        var host = Environment.GetEnvironmentVariable("RabbitMQ__Host") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("RabbitMQ__Port") ?? "5672";
        var username = Environment.GetEnvironmentVariable("RabbitMQ__Username") ?? "guest";
        var password = Environment.GetEnvironmentVariable("RabbitMQ__Password") ?? "guest";

        var connectionString = $"host={host};port={port};username={username};password={password}";

        try
        {
            using var bus = RabbitHutch.CreateBus(connectionString);
            Console.WriteLine("Connected to RabbitMQ");

            bus.PubSub.Subscribe<TextMessage>("subscriber", HandleTextMessage);
            Console.WriteLine("Listening for messages. Press Ctrl+C to exit.");

            var cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Delay(-1, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Shutting down...");
            }
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Task was canceled: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    static Task HandleTextMessage(TextMessage textMessage)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Got message: {0}", textMessage.Text);
        Console.ResetColor();
        return Task.CompletedTask;
    }
}