// using Agents.Configuration;
// using Agents.Interfaces;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using Subscriber;
//
// namespace Agents.Module;
//
// public static class ServiceCollectionExtensions
// {
//     public static void AddServiceAgents(this IServiceCollection services, IConfiguration configuration)
//     {
//         services.AddMinIoServices(configuration);
//
//         var elasticSearchConfig = configuration.GetSection("ElasticSearch:Connection").Get<ElasticSearchConfig>();
//         services.AddSingleton<IElasticSearchServiceAgent>(serviceProvider =>
//         {
//             var logger = serviceProvider.GetRequiredService<ILogger<ElasticSearch>>();
//             logger.LogInformation("Initializing ElasticSearch client...");
//             logger.LogInformation("ElasticSearch configured with URL: {Url}, Default Index: {DefaultIndex}",
//                 elasticSearchConfig?.Url, elasticSearchConfig?.DefaultIndex);
//
//             return new ElasticSearch(elasticSearchConfig?.Url, elasticSearchConfig?.DefaultIndex);
//         });
//
//         var rabbitMqConfig = configuration.GetSection("RabbitMQ").Get<RabbitMqConfig>();
//         services.AddSingleton(serviceProvider =>
//         {
//             var logger = serviceProvider.GetRequiredService<ILogger<SubscriberService>>();
//             logger.LogInformation("Initializing RabbitMQ client...");
//             logger.LogInformation("RabbitMQ configured with Host: {Host}, Port: {Port}",
//                 rabbitMqConfig?.Host, rabbitMqConfig!.Port);
//
//             return new SubscriberService(rabbitMqConfig.GetConnectionString());
//         });
//     }
// }