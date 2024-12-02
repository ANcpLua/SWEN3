using Contract.DTOModels;
using EasyNetQ;
using FluentValidation;
using Mapper;
using PaperlessREST.Validation;
using PaperlessService.Entities;
using PaperlessService.Module;
using PaperlessService.Validation;
using PostgreSQL.Data;
using PostgreSQL.Entities;
using PostgreSQL.Module;
using PostgreSQL.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Mapping));

// Validation 
builder.Services.AddScoped<IValidator<Document>, DocumentEntityValidator>();
builder.Services.AddScoped<IValidator<BlDocument>, BlValidation>();
builder.Services.AddScoped<IValidator<DocumentUploadDto>, DocumentUploadDtoValidator>();

// Database configuration
builder.Services.AddPostgreSqlServices(builder.Configuration);
// Services configuration
builder.Services.AddBusinessLogicServices();

// Message queue configuration
builder.Services.AddSingleton(RabbitHutch.CreateBus(
    $"host={builder.Configuration["RabbitMQ:Host"] ?? "localhost"};" +
    $"port={builder.Configuration["RabbitMQ:Port"] ?? "5672"};" +
    $"username={builder.Configuration["RabbitMQ:Username"] ?? "guest"};" +
    $"password={builder.Configuration["RabbitMQ:Password"] ?? "guest"}"
));

var app = builder.Build();

// Database initialization and seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PaperlessDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try 
    {
        await DatabaseSeeder.SeedDatabase(context);
        logger.LogInformation("Database seeded successfully");
        
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsPath);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the application");
        throw;
    }
}

// Development-specific middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Security and routing middleware
app.UseHttpsRedirection();
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// API endpoints
app.MapControllers();
// SPA fallback route for client-side routing
app.MapFallbackToFile("index.html");

app.Run();