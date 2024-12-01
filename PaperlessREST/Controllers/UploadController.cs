using Contract.DTOModels;
using Microsoft.AspNetCore.Mvc;
using PaperlessService.InterfacesBL;

namespace PaperlessREST.Controllers;

[ApiController]
[Route("api/documents/upload")]
public class UploadController : ControllerBase
{
    private readonly IUploadService _uploadService;

    public UploadController(IUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    [HttpPost]
    public async Task<ActionResult<DocumentDto>> Upload([FromForm] DocumentUploadDto uploadDto)
    {
        try
        {
            var document = await _uploadService.Upload(uploadDto);
            return CreatedAtAction(nameof(Upload), new { id = document.Id }, document);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}