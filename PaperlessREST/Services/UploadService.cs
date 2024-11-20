using AutoMapper;
using FluentValidation;
using PaperlessREST.DomainModel;
using PaperlessREST.Models;
using PostgreSQL.Persistence;

namespace PaperlessREST.Services;

public class UploadService : IUploadService
{
    private readonly IDocumentRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<Document> _validator;
    private readonly ILogger<UploadService> _logger;

    public UploadService(
        IDocumentRepository repository,
        IMapper mapper,
        IValidator<Document> validator,
        ILogger<UploadService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<DocumentDto> Upload(DocumentUploadDto uploadDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting upload for document titled '{Title}'", uploadDto.Title);

        // Map UploadDto to Domain Model
        var document = _mapper.Map<Document>(uploadDto);

        // Save the file to disk
        var fileName = $"{Guid.NewGuid()}_{uploadDto.File.FileName}";
        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadsDir);
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await uploadDto.File.CopyToAsync(stream, cancellationToken);
        }

        document.FilePath = filePath;

        // Validate the document
        var validationResult = await _validator.ValidateAsync(document, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for document titled '{Title}': {Errors}",
                uploadDto.Title,
                string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

            throw new ValidationException(validationResult.Errors);
        }

        // Map to Entity Model and save to DB via repository
        var entity = _mapper.Map<PostgreSQL.Entities.Document>(document);
        await _repository.UploadAsync(entity, cancellationToken);

        _logger.LogInformation("Document titled '{Title}' uploaded successfully with ID {Id}", uploadDto.Title,
            entity.Id);

        // Map back to DTO
        var result = _mapper.Map<DocumentDto>(document);
        result.Id = entity.Id; // Ensure the ID is set

        return result;
    }
}

