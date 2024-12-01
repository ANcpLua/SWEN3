using Microsoft.AspNetCore.Mvc;
using PaperlessService.InterfacesBL;

namespace PaperlessREST.Controllers;

[ApiController]
[Route("api/documents")]
public class DeleteDocumentController : ControllerBase
{
    private readonly IDeleteDocumentService _deleteService;

    public DeleteDocumentController(IDeleteDocumentService deleteService)
    {
        _deleteService = deleteService;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _deleteService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}