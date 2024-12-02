using Contract.DTOModels;

namespace PaperlessService.InterfacesBL;

public interface IUploadService
{
    Task<DocumentDto> Upload(DocumentUploadDto document, CancellationToken cancellationToken = default);
}