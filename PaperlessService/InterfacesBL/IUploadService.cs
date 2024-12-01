using Contract.DTOModels;
using PaperlessService.Entities;

namespace PaperlessService.InterfacesBL;

public interface IUploadService
{
    Task<BlDocument> Upload(DocumentUploadDto uploadDto, CancellationToken cancellationToken = default);
}