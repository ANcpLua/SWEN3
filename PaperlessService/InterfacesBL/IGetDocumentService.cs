using PaperlessService.Entities;

namespace PaperlessService.InterfacesBL;

public interface IGetDocumentService
{
    Task<BlDocument?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BlDocument>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(string FilePath, string ContentType)> GetFileAsync(int id, CancellationToken cancellationToken = default);
}