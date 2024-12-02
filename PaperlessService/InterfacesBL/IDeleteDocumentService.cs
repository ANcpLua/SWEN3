namespace PaperlessService.InterfacesBL;

public interface IDeleteDocumentService
{
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);
}