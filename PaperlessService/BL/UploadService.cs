using AutoMapper;
using Contract.DTOModels;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PaperlessService.Entities;
using PaperlessService.InterfacesBL;
using PostgreSQL.Entities;
using PostgreSQL.Persistence;

namespace PaperlessService.BL;

public class UploadService : IUploadService
{
    private readonly IDocumentRepository _repository;
    private readonly IValidator<BlDocument> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<UploadService> _logger;

    public UploadService(
        IDocumentRepository repository,
        IValidator<BlDocument> validator,
        IMapper mapper,
        ILogger<UploadService> logger)
    {
        _repository = repository;
        _validator = validator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DocumentDto> Upload(DocumentUploadDto uploadDto, CancellationToken cancellationToken = default)
    {
        // Step 1: Map UploadDto to Domain Model
        var document = _mapper.Map<BlDocument>(uploadDto);
        document.DateUploaded = DateTime.UtcNow;

        // Step 2: Generate and Assign File Path
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(uploadDto.File.FileName)}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);
        document.FilePath = filePath; // Assign the generated file path to the document before validation

        // Step 3: Validate Document Entity
        var validationResult = await _validator.ValidateAsync(document, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Document validation failed: {Errors}", validationResult.Errors);
            throw new ValidationException(validationResult.Errors);
        }

        // Step 4: Save File to Disk
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadDto.File.CopyToAsync(stream, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving the uploaded file");
            throw;
        }

        // Step 5: Persist Document Data to Database
        var entityDocument = _mapper.Map<Document>(document);
        try
        {
            var uploadedEntityDocument = await _repository.Upload(entityDocument, cancellationToken);
            var uploadedDocument = _mapper.Map<BlDocument>(uploadedEntityDocument);

            _logger.LogInformation($"Document with ID {uploadedDocument.Id} uploaded successfully");

            // Step 6: Map to DTO and Return
            return _mapper.Map<DocumentDto>(uploadedDocument);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while saving the document entity to the database");
            throw;
        }
    }
}