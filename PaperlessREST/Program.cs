using System.Security.Cryptography.X509Certificates;
using Agents;
using Agents.Configuration;
using Agents.Interfaces;
using Mapper;
using Publisher;
using Subscriber;
using Agents.Module;
using PaperlessService.Entities;
using PaperlessService.Validation;
using FluentValidation;
using OcrWorker.Module;
using PaperlessService.Module;
using PostgreSQL.Module;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Core service registration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Mapping));

// Configure logging with proper typed loggers
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddSingleton<PublisherService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<PublisherService>>();
    var rabbitMqHost = configuration.GetValue<string>("RabbitMQ:Host");
    var rabbitMqPort = configuration.GetValue<string>("RabbitMQ:Port");
    var rabbitMqUsername = configuration.GetValue<string>("RabbitMQ:Username");
    var rabbitMqPassword = configuration.GetValue<string>("RabbitMQ:Password");

    // Construct the RabbitMQ connection string
    var connectionString = $"amqp://{rabbitMqUsername}:{rabbitMqPassword}@{rabbitMqHost}:{rabbitMqPort}";
    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("RabbitMQ connection string is not configured");

    logger.LogInformation("RabbitMQ connection string: {ConnectionString}", connectionString);

    return new PublisherService(connectionString, logger);
});

builder.Services.AddSingleton<SubscriberService>(provider =>
{
    var rabbitMqHost = configuration.GetValue<string>("RabbitMQ:Host");
    var rabbitMqPort = configuration.GetValue<string>("RabbitMQ:Port");
    var rabbitMqUsername = configuration.GetValue<string>("RabbitMQ:Username");
    var rabbitMqPassword = configuration.GetValue<string>("RabbitMQ:Password");

    // Construct the RabbitMQ connection string
    var connectionString = $"amqp://{rabbitMqUsername}:{rabbitMqPassword}@{rabbitMqHost}:{rabbitMqPort}";
    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("RabbitMQ connection string is not configured");

    return new SubscriberService(connectionString);
});

// Register all module services
builder.Services.AddPostgreSqlServices(configuration);
var elasticSearchConfig = configuration.GetSection("ElasticSearch:Connection").Get<ElasticSearchConfig>();

elasticSearchConfig?.Validate(); 

builder.Services.AddSingleton<IElasticSearchServiceAgent>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<ElasticSearch>>();
    logger.LogInformation("Initializing ElasticSearch client...");
    logger.LogInformation("ElasticSearch configured with URL: {Url}, Default Index: {DefaultIndex}",
        elasticSearchConfig?.Url, elasticSearchConfig?.DefaultIndex);

    return new ElasticSearch(elasticSearchConfig?.Url, elasticSearchConfig?.DefaultIndex);
});

builder.Services.AddBusinessLogicServices();
builder.Services.AddOcrServices(configuration);

// Register validation
builder.Services.AddScoped<IValidator<BlDocument>, DocumentValidator>();
builder.Services.AddServiceAgents(configuration);
// Configure CORS
builder.Services.AddCors(options => 
{ 
    options.AddPolicy("AllowAll", policy => 
    { 
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod(); 
    }); 
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var certPath = configuration.GetValue<string>("Kestrel:Endpoints:Https:Certificate:Path");
    var certPass = configuration.GetValue<string>("Kestrel:Endpoints:Https:Certificate:Password");

    if (!string.IsNullOrEmpty(certPath) && File.Exists(certPath) && !string.IsNullOrEmpty(certPass))
    {
        serverOptions.ConfigureHttpsDefaults(httpsOptions =>
        {
            httpsOptions.ServerCertificate = new X509Certificate2(certPath, certPass);
        });
    }
    else
    {
        Console.WriteLine($"Certificate not found or invalid path: {certPath}");
        throw new FileNotFoundException("The HTTPS certificate file was not found.", certPath);
    }
});

var app = builder.Build();

if (configuration.GetValue<bool>("Swagger:EnableSwagger"))
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => 
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Paperless API v1")
    );
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

try
{
    await app.Services.InitializeDatabaseAsync();
    await app.RunAsync();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Application startup failed with exception");
    throw;
}