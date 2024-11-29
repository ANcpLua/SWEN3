using Microsoft.EntityFrameworkCore;
using PostgreSQL.Data;
using PostgreSQL.Entities;

namespace PostgreSQL.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedDatabase(PaperlessDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        
        if (!await context.Documents.AnyAsync())
        {
            var sampleDocuments = new List<Document>
            {
                new()
                {
                    Name = "Sample PDF Document",
                    FilePath = Path.Combine("wwwroot", "uploads", "sample.pdf"),
                    DateUploaded = DateTime.UtcNow.AddDays(-5)
                },
                new()
                {
                    Name = "Test Document",
                    FilePath = Path.Combine("wwwroot", "uploads", "test.docx"),
                    DateUploaded = DateTime.UtcNow.AddDays(-3)
                }
            };

            await context.Documents.AddRangeAsync(sampleDocuments);
            await context.SaveChangesAsync();
        }
    }
}