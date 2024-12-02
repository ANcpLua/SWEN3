using Microsoft.Extensions.DependencyInjection;
using PaperlessService.BL;
using PaperlessService.InterfacesBL;

namespace PaperlessService.Module;

public static class BlExtension
{
    public static void AddBusinessLogicServices(this IServiceCollection services)
    {
        services.AddScoped<IUploadService, UploadService>();
        services.AddScoped<IGetDocumentService, GetDocumentService>();
        services.AddScoped<IDeleteDocumentService, DeleteDocumentService>();
    }
}