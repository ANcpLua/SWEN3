using Messages;
using Microsoft.Extensions.Logging;
using PaperlessService.InterfacesBL;
using PostgreSQL.Persistence;

namespace PaperlessService.BL;

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

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entityDocument = await _repository.GetByIdAsync(id, cancellationToken);
        if (entityDocument != null && File.Exists(entityDocument.FilePath))
        {
            File.Delete(entityDocument.FilePath);
        }

        await _repository.DeleteAsync(id, cancellationToken);
    }
    
    public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        var entityDocuments = await _repository.GetAllAsync(cancellationToken);
        foreach (var entityDocument in entityDocuments)
        {
            if (File.Exists(entityDocument.FilePath))
            {
                File.Delete(entityDocument.FilePath);
            }
        }

        await _repository.DeleteAllAsync(cancellationToken);
    }
}