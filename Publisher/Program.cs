using EasyNetQ;
using Messages;

namespace Publisher;

class Program
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
            Console.WriteLine("Enter a message. 'Quit' to quit.");
            
            string? input;
            while ((input = Console.ReadLine()) != "Quit")
            {
                if (!string.IsNullOrEmpty(input))
                {
                    await bus.PubSub.PublishAsync(new TextMessage { Text = input });
                    Console.WriteLine("Message published!");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }
}