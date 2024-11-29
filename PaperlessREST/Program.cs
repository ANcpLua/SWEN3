using EasyNetQ;
using FluentValidation;
using PaperlessREST.AutoMapper;
using PaperlessREST.Models;
using PaperlessREST.Services;
using PaperlessREST.Validation;
using PostgreSQL.Data;
using PostgreSQL.Entities;
using PostgreSQL.Module;
using PostgreSQL.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(DocumentProfile));

// Service registrations
builder.Services.AddScoped<IValidator<Document>, DocumentEntityValidator>();
builder.Services.AddScoped<IValidator<PaperlessREST.DomainModel.Document>, DocumentValidator>();
builder.Services.AddScoped<IValidator<DocumentUploadDto>, DocumentUploadDtoValidator>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IGetDocumentService, GetDocumentService>();
builder.Services.AddScoped<IDeleteDocumentService, DeleteDocumentService>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// Database configuration
builder.Services.AddPostgreSqlServices(builder.Configuration);

// Message queue configuration
builder.Services.AddSingleton<IBus>(RabbitHutch.CreateBus(
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