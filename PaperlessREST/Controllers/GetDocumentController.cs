using Contract.DTOModels;
using Microsoft.AspNetCore.Mvc;
using PaperlessService.InterfacesBL;

namespace PaperlessREST.Controllers;

[ApiController]
[Route("api/documents")]
public class GetDocumentController : ControllerBase
{
    private readonly IGetDocumentService _service;

    public GetDocumentController(IGetDocumentService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDto>> GetById(int id)
    {
        var document = await _service.GetByIdAsync(id);
        if (document == null)
            return NotFound(new { Message = $"Document with ID {id} not found" });

        var documentDto = new DocumentDto
        {
            Id = document.Id,
            Name = document.Name,
            FilePath = document.FilePath,
            DateUploaded = document.DateUploaded
        };

        return Ok(documentDto);
    }

    [HttpGet("{id}/content")]
    public async Task<IActionResult> GetContent(int id)
    {
        try
        {
            var (filePath, contentType) = await _service.GetFileAsync(id, HttpContext.RequestAborted);
            var fileStream = System.IO.File.OpenRead(filePath);
            return File(fileStream, contentType);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { e.Message });
        }
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<DocumentDto>>> GetAll()
    {
        var documents = await _service.GetAllAsync();
        var documentDtos = documents.Select(doc => new DocumentDto
        {
            Id = doc.Id,
            Name = doc.Name,
            FilePath = doc.FilePath,
            DateUploaded = doc.DateUploaded
        });

        return Ok(documentDtos);
    }
}