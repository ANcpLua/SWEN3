using AutoMapper;
using Microsoft.Extensions.Logging;
using PaperlessService.Entities;
using PaperlessService.InterfacesBL;
using PostgreSQL.Persistence;
using Publisher;

namespace PaperlessService.BL;

public class GetDocumentService : IGetDocumentService
{
    private readonly IMapper _mapper;
    private readonly PublisherService _publisherService;
    private readonly IDocumentRepository _repository;
    private readonly ILogger<GetDocumentService> _logger;

    public GetDocumentService(
        IDocumentRepository repository, 
        IMapper mapper, 
        PublisherService publisherService,
        ILogger<GetDocumentService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _publisherService = publisherService;
        _logger = logger;
    }

    public async Task<BlDocument?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await _repository.GetByIdAsync(id, cancellationToken);
            await _publisherService.PublishEventAsync(id.ToString(), document?.FilePath ?? string.Empty, DateTime.UtcNow);
            return document != null ? _mapper.Map<BlDocument>(document) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get document: {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<BlDocument>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var documents = await _repository.GetAllAsync(cancellationToken);
            await _publisherService.PublishEventAsync("all", "N/A", DateTime.UtcNow);
            return _mapper.Map<IEnumerable<BlDocument>>(documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all documents");
            throw;
        }
    }

    public async Task<(string FilePath, string ContentType)> GetFileAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var document = await GetByIdAsync(id, cancellationToken);
            await _publisherService.PublishEventAsync(id.ToString(), document!.FilePath, DateTime.UtcNow);
            return (document.FilePath, GetContentType(document.FilePath));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get file: {Id}", id);
            throw;
        }
    }

    private static string GetContentType(string filePath) => Path.GetExtension(filePath).ToLower() switch
    {
        ".pdf" => "application/pdf",
        ".doc" => "application/msword",
        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        _ => "application/octet-stream"
    };
}