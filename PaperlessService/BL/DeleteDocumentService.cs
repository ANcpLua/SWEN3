using PaperlessService.InterfacesBL;
using PostgreSQL.Persistence;
using Publisher;

namespace PaperlessService.BL;

public class DeleteDocumentService : IDeleteDocumentService
{
    private readonly PublisherService _publisherService;
    private readonly IDocumentRepository _repository;

    public DeleteDocumentService(
        IDocumentRepository repository,
        PublisherService publisherService)
    {
        _repository = repository;
        _publisherService = publisherService;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var document = await _repository.GetByIdAsync(id, cancellationToken);
        if (document == null) return;

        if (File.Exists(document.FilePath)) File.Delete(document.FilePath);

        await _repository.DeleteAsync(id, cancellationToken);

        await _publisherService.PublishAsync(new
        {
            DocumentId = id,
            Action = "Delete",
            Details = $"Document deleted: {document.Name}",
            Timestamp = DateTime.UtcNow
        });
    }
}