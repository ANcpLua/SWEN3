using PaperlessREST.Models;

namespace PaperlessREST.Services;

public interface IGetDocumentService
{
    Task<DocumentDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<DocumentDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<DocumentDto> GetMetadataAsync(int id, CancellationToken cancellationToken); 
    Task<(string FilePath, string ContentType)> GetFileAsync(int id, CancellationToken cancellationToken);
}