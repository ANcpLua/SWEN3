using AutoMapper;
using PaperlessREST.Models;
using PostgreSQL.Persistence;

namespace PaperlessREST.Services;

public class GetDocumentService : IGetDocumentService
{
    private readonly IDocumentRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDocumentService> _logger;

    public GetDocumentService(
        IDocumentRepository repository,
        IMapper mapper,
        ILogger<GetDocumentService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DocumentDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving document with ID {Id}", id);
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
        {
            _logger.LogWarning("Document with ID {Id} not found", id);
            return null;
        }

        var domainModel = _mapper.Map<DomainModel.Document>(entity);
        return _mapper.Map<DocumentDto>(domainModel);
    }

    public async Task<IEnumerable<DocumentDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving all documents");
        var entities = await _repository.GetAllAsync(cancellationToken);
        var domainModels = _mapper.Map<IEnumerable<DomainModel.Document>>(entities);
        return _mapper.Map<IEnumerable<DocumentDto>>(domainModels);
    }

    public async Task<DocumentDto> GetMetadataAsync(int id, CancellationToken cancellationToken)
    {
        var document = await GetByIdAsync(id, cancellationToken);
        if (document == null)
            throw new KeyNotFoundException($"Document {id} not found");
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

        return (absolutePath, contentType);
    }
}

