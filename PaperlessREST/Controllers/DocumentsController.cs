using Microsoft.AspNetCore.Mvc;
using PaperlessREST.Models;
using PaperlessREST.Services;

namespace PaperlessREST.Controllers;

[ApiController] 
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    private readonly IGetDocumentService _getService;
    private readonly IDeleteDocumentService _deleteService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IGetDocumentService getService,
        IDeleteDocumentService deleteService, 
        ILogger<DocumentsController> logger)
    {
        _getService = getService;
        _deleteService = deleteService;
        _logger = logger;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try 
        {
            await _deleteService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, "Request canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {Id}", id);
            return StatusCode(500, "An error occurred while deleting the document");
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var documents = await _getService.GetAllAsync(cancellationToken);
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
            var document = await _getService.GetByIdAsync(id, cancellationToken);
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
            var (filePath, contentType) = await _getService.GetFileAsync(id, cancellationToken);
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
            var document = await _getService.GetMetadataAsync(id, cancellationToken);

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
