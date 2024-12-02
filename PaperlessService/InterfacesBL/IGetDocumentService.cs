using Contract.DTOModels;

namespace PaperlessService.InterfacesBL;

public interface IGetDocumentService
{
    Task<DocumentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DocumentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DocumentDto> GetMetadataAsync(int id, CancellationToken cancellationToken);
    Task<(string FilePath, string ContentType)> GetFileAsync(int id, CancellationToken cancellationToken);
}