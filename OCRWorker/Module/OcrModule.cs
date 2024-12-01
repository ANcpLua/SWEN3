using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OcrWorker.Tesseract;

namespace OcrWorker.Module;

public static class OcrModule
{
    public static void AddOcrServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OcrOptions>(configuration.GetSection("OCR"));
        services.AddSingleton<IOcrClient>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<OcrOptions>>().Value;
            return new Ocr(options);
        });

        services.AddSingleton<IOcrClient>(_ => new Ocr( new OcrOptions
        {
            TessDataPath = configuration.GetValue<string>("OCR:TesseractPath"),
            Language = configuration.GetValue<string>("OCR:Language"),
        }));
    }
}