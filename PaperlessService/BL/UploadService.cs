using Agents.Interfaces;
using AutoMapper;
using Contract.DTOModels;
using Contract.Messages;
using Microsoft.Extensions.Logging;
using PaperlessService.Entities;
using PaperlessService.InterfacesBL;
using PostgreSQL.Entities;
using PostgreSQL.Persistence;
using Publisher;

namespace PaperlessService.BL;

public class UploadService : IUploadService
{
    private readonly IElasticSearchServiceAgent _elasticSearchService;
    private readonly IMapper _mapper;
    private readonly IMinIoServiceAgent _minIoService;
    private readonly PublisherService _publisherService;
    private readonly IDocumentRepository _repository;
    private readonly ILogger<UploadService> _logger;

   public UploadService(
        IDocumentRepository repository,
        IMinIoServiceAgent minIoService,
        IElasticSearchServiceAgent elasticSearchService,
        PublisherService publisherService,
        IMapper mapper,
        ILogger<UploadService> logger)
    {
        _repository = repository;
        _minIoService = minIoService;
        _elasticSearchService = elasticSearchService;
        _publisherService = publisherService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BlDocument> Upload(DocumentUploadDto uploadDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var uniqueFileName = await SaveFileToMinIo(uploadDto);
            _logger.LogInformation("File saved to MinIO: {FileName}", uniqueFileName);

            // Save metadata to database
            var savedDocument = await SaveMetadataToDatabase(uploadDto, uniqueFileName, cancellationToken);
            _logger.LogInformation("Metadata saved to database for document: {DocumentId}", savedDocument.Id);

            // Index in ElasticSearch
            try
            {
                await _elasticSearchService.IndexAsync(savedDocument);
                _logger.LogInformation("Document indexed in ElasticSearch: {DocumentId}", savedDocument.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to index document in ElasticSearch. Document ID: {DocumentId}", savedDocument.Id);
            }

            // Publish events
            try
            {
                await PublishEventToRabbitMq(savedDocument);
                await PublishOcrEvent(savedDocument, uploadDto.File.FileName);
                _logger.LogInformation("Events published for document: {DocumentId}", savedDocument.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish events for document: {DocumentId}", savedDocument.Id);
            }

            return _mapper.Map<BlDocument>(savedDocument);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process document upload: {FileName}", uploadDto?.File?.FileName);
            throw;
        }
    }

    private async Task PublishOcrEvent(Document savedDocument, string originalFileName)
    {
        var ocrMessage = new OcrProcessingMessage
        {
            DocumentId = savedDocument.Id.ToString(),
            FilePath = savedDocument.FilePath,
            Timestamp = DateTime.UtcNow,
            OriginalFileName = originalFileName
        };

        try
        {
            await _publisherService.PublishAsync(ocrMessage);
            _logger.LogInformation("Published OCR processing request for document: {DocumentId}", savedDocument.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish OCR processing request for document: {DocumentId}", savedDocument.Id);
        }
    }
    
    private async Task<string> SaveFileToMinIo(DocumentUploadDto uploadDto)
    {
        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(uploadDto.File.FileName);
        using var stream = uploadDto.File.OpenReadStream();
        await _minIoService.UploadFileAsync("uploads", uniqueFileName, stream);
        return uniqueFileName;
    }

    private async Task<Document> SaveMetadataToDatabase(DocumentUploadDto uploadDto, string uniqueFileName, CancellationToken cancellationToken)
    {
        var document = _mapper.Map<BlDocument>(uploadDto);
        document.FilePath = uniqueFileName;
        document.DateUploaded = DateTime.UtcNow;

        var documentEntity = _mapper.Map<Document>(document);
        return await _repository.Upload(documentEntity, cancellationToken);
    }

    private async Task PublishEventToRabbitMq(Document savedDocument)
    {
        await _publisherService.PublishAsync(new
        {
            DocumentId = savedDocument.Id, Action = "Upload", Details = $"Uploaded document: {savedDocument.Name}",
            TimeStamp = DateTime.UtcNow
        });
    }
}