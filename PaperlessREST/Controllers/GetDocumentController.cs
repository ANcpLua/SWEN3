using Contract.DTOModels;
using Microsoft.AspNetCore.Mvc;
using PaperlessService.InterfacesBL;

namespace PaperlessREST.Controllers;

[ApiController]
[Route("api/documents")]
public class GetDocumentController : ControllerBase
{
    private readonly IGetDocumentService _service;
    private readonly ILogger<GetDocumentController> _logger;

    public GetDocumentController(
        IGetDocumentService service,
        ILogger<GetDocumentController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("{id}/content")]
    public async Task<IActionResult> GetContent(int id, CancellationToken cancellationToken)
    {
        try
        {
            var (filePath, contentType) = await _service.GetFileAsync(id, cancellationToken);
            if (string.IsNullOrEmpty(filePath))
            {
                return NotFound("Content file not found for this document.");
            }
            return PhysicalFile(filePath, contentType);
        }
        catch (KeyNotFoundException)
        {
            _logger.LogWarning("Document {Id} not found.", id);
            return NotFound("Document not found.");
        }
        catch (FileNotFoundException)
        {
            _logger.LogError("File not found for document {Id}.", id);
            return NotFound("File not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file for document {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the file");
        }
    }
    [HttpGet("{id}/metadata")]
    public async Task<ActionResult<DocumentDto>> GetMetadata(int id, CancellationToken cancellationToken)
    {
        try
        {
            var document = await _service.GetMetadataAsync(id, cancellationToken);
            if (document == null)
            {
                return NotFound("Document not found.");
            }

            return Ok(document); 
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Document not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata for document {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the metadata");
        }
    }

    [HttpGet ("/all")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var documents = await _service.GetAllAsync(cancellationToken);
            return Ok(documents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all documents");
            return StatusCode(500, "An error occurred while retrieving documents");
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var document = await _service.GetByIdAsync(id, cancellationToken);
            if (document == null)
            {
                return NotFound("Document not found.");
            }

            return Ok(document);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Document not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the document");
        }
    }
}