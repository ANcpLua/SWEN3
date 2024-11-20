using PostgreSQL.Persistence;

namespace PaperlessREST.Services;

public class DeleteDocumentService : IDeleteDocumentService
{
    private readonly IDocumentRepository _repository;
    private readonly ILogger<DeleteDocumentService> _logger;

    public DeleteDocumentService(
        IDocumentRepository repository,
        ILogger<DeleteDocumentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var document = await _repository.GetByIdAsync(id, cancellationToken);
        if (document == null) throw new KeyNotFoundException($"Document {id} not found");

        // Delete physical file if exists
        if (File.Exists(document.FilePath))
        {
            File.Delete(document.FilePath);
        }

        // Delete from database
        await _repository.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("Document {Id} deleted successfully", id);
    }
}