using PaperlessService.Entities;
using PostgreSQL.Entities;

namespace Tests.TestHelper;

public static class TestFramework
{
    public static class TestDataFactory
    {
        public static class Documents
        {
            public static Document CreateDocument(int id, string? name = null, string? filePath = null,
                DateTime? dateUploaded = null) =>
                new()
                {
                    Id = id, Name = name ?? $"Test Document {id}", FilePath = filePath ?? $"/path/to/document_{id}.pdf",
                    DateUploaded = dateUploaded ?? DateTime.UtcNow
                };

            public static BlDocument CreateBlDocument(Document source) =>
                new()
                {
                    Id = source.Id, Name = source.Name, FilePath = source.FilePath, DateUploaded = source.DateUploaded
                };
        }
    }
}
