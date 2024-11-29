using AutoMapper;
using PaperlessREST.DomainModel;
using PaperlessREST.Models;
using PostgreSQL.Persistence;
using EasyNetQ;
using Messages;
using PaperlessREST.DTOModels;

namespace PaperlessREST.Services;

public class GetDocumentService : IGetDocumentService
{
    private readonly IDocumentRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDocumentService> _logger;
    private readonly IBus _bus;

    public GetDocumentService(
        IDocumentRepository repository,
        IMapper mapper,
        ILogger<GetDocumentService> logger,
        IBus bus)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _bus = bus;
    }

    public async Task<DocumentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entityDocument = await _repository.GetByIdAsync(id, cancellationToken);
        if (entityDocument == null) return null;
            
        var document = _mapper.Map<Document>(entityDocument);
        var documentDto = _mapper.Map<DocumentDto>(document);

        await PublishDocumentAction(new DocumentActionMessage
        {
            DocumentId = id,
            Action = "GetById",
            Details = $"Retrieved document {id}",
            Timestamp = DateTime.UtcNow
        }, cancellationToken);

        return documentDto;
    }

    public async Task<IEnumerable<DocumentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entityDocuments = await _repository.GetAllAsync(cancellationToken);
        var documents = _mapper.Map<IEnumerable<Document>>(entityDocuments);
        var documentDtos = _mapper.Map<IEnumerable<DocumentDto>>(documents);

        await PublishDocumentAction(new DocumentActionMessage
        {
            Action = "GetAll",
            Details = $"Retrieved {documentDtos.Count()} documents",
            Timestamp = DateTime.UtcNow
        }, cancellationToken);

        return documentDtos;
    }
    
    public async Task<DocumentDto> GetMetadataAsync(int id, CancellationToken cancellationToken)
    {
        var document = await GetByIdAsync(id, cancellationToken);
        if (document == null)
            throw new KeyNotFoundException($"Document {id} not found");

        await PublishDocumentAction(new DocumentActionMessage
        {
            DocumentId = id,
            Action = "GetMetadata",
            Details = $"Retrieved metadata for document {id}",
            Timestamp = DateTime.UtcNow
        }, cancellationToken);

        return document;
    }

    public async Task<(string FilePath, string ContentType)> GetFileAsync(int id, CancellationToken cancellationToken)
    {
        var document = await GetByIdAsync(id, cancellationToken);
        if (document == null)
            throw new KeyNotFoundException($"Document {id} not found");

        var absolutePath = Path.GetFullPath(document.FilePath);
        if (!System.IO.File.Exists(absolutePath))
            throw new FileNotFoundException($"File not found: {absolutePath}");

        var contentType = Path.GetExtension(absolutePath).ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };

        await PublishDocumentAction(new DocumentActionMessage
        {
            DocumentId = id,
            Action = "GetFile",
            Details = $"Retrieved file content for document {id}",
            Timestamp = DateTime.UtcNow
        }, cancellationToken);

        return (absolutePath, contentType);
    }

    private async Task PublishDocumentAction(DocumentActionMessage message, CancellationToken cancellationToken)
    {
        try
        {
            await _bus.PubSub.PublishAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish document action message: {Action} for document {DocumentId}", 
                message.Action, message.DocumentId);
        }
    }
}