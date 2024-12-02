// using Agents.Configuration;
// using Agents.Interfaces;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
//
// namespace Agents.Module;
//
// public static class MinIoExtensions
// {
//     public static void AddMinIoServices(this IServiceCollection services, IConfiguration configuration)
//     {
//         services.Configure<MinIoOptions>(configuration.GetSection("MinIO"));
//         services.AddSingleton<IMinIoServiceAgent>(serviceProvider =>
//         {
//             var options = serviceProvider.GetRequiredService<IOptions<MinIoOptions>>().Value;
//             var logger = serviceProvider.GetRequiredService<ILogger<MinIo>>();
//
//             logger.LogInformation("Initializing MinIO client...");
//             logger.LogInformation("MinIO configured with Endpoint: {Endpoint}", options.Endpoint);
//
//             return new MinIo(
//                 new OptionsWrapper<MinIoOptions>(options),
//                 logger
//             );
//         });
//     }
// }