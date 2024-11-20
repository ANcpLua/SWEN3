using PaperlessREST.Models;

namespace PaperlessREST.Services
{
    public interface IUploadService
    {
        Task<DocumentDto> Upload(DocumentUploadDto uploadDto, CancellationToken cancellationToken);
    }
}