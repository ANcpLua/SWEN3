using Microsoft.AspNetCore.Mvc;
using PaperlessREST.DTOModels;
using PaperlessREST.Models;
using PaperlessREST.Services;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        try
        {
            var documents = await _service.GetAllAsync(cancellationToken);
            return Ok(documents);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, "Request canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving documents");
            return StatusCode(500, "An error occurred while retrieving documents");
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDto>> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var document = await _service.GetByIdAsync(id, cancellationToken);
            if (document == null) return NotFound();
            return Ok(document);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, "Request canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the document");
        }
    }

    [HttpGet("{id}/content")]
    public async Task<IActionResult> GetContent(int id, CancellationToken cancellationToken)
    {
        try
        {
            var (filePath, contentType) = await _service.GetFileAsync(id, cancellationToken);
            return PhysicalFile(filePath, contentType);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (FileNotFoundException)
        {
            return NotFound("File not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving file for document {Id}", id);
            return StatusCode(500);
        }
    }

    [HttpGet("{id}/metadata")]
    public async Task<IActionResult> GetMetadata(int id, CancellationToken cancellationToken)
    {
        try
        {
            var document = await _service.GetMetadataAsync(id, cancellationToken);

            return Ok(new
            {
                document.Id,
                document.Name,
                document.FilePath,
                document.DateUploaded,
                FileSize = new FileInfo(document.FilePath).Length,
                FileType = Path.GetExtension(document.FilePath)
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving metadata for document {Id}", id);
            return StatusCode(500);
        }
    }
}
