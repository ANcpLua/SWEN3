// using Agents.Interfaces;
// using Contract.Messages;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
// using OcrWorker.Tesseract;
// using Subscriber;
//
// namespace OcrWorker.Service;
//
// public class OcrWorkerService : BackgroundService
// {
//     private readonly ILogger<OcrWorkerService> _logger;
//     private readonly IOcrClient _ocrClient;
//     private readonly IMinIoServiceAgent _minIoService;
//     private readonly IElasticSearchServiceAgent _elasticSearchService;
//     private readonly SubscriberService _subscriberService;
//
//     public OcrWorkerService(
//         ILogger<OcrWorkerService> logger,
//         IOcrClient ocrClient,
//         IMinIoServiceAgent minIoService,
//         IElasticSearchServiceAgent elasticSearchService,
//         SubscriberService subscriberService)
//     {
//         _logger = logger;
//         _ocrClient = ocrClient;
//         _minIoService = minIoService;
//         _elasticSearchService = elasticSearchService;
//         _subscriberService = subscriberService;
//     }
//
//     protected override Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         // Log the service startup
//         _logger.LogInformation("OCR Worker Service starting at: {Time}", DateTimeOffset.Now);
//
//         try
//         {
//             // Subscribe to OCR processing messages
//             _subscriberService.Subscribe<OcrProcessingMessage>("OCRProcessor", async void (message) =>
//             {
//                 try
//                 {
//                     await ProcessOcrMessageAsync(message, stoppingToken);
//                 }
//                 catch (Exception ex)
//                 {
//                     _logger.LogError(ex, "Error processing OCR message for document {DocumentId}", message.DocumentId);
//                 }
//             });
//
//             _logger.LogInformation("Successfully subscribed to OCR processing queue");
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Failed to subscribe to OCR processing queue");
//           
//         }
//
//         return Task.CompletedTask;
//     }
//
//     private async Task ProcessOcrMessageAsync(OcrProcessingMessage message, CancellationToken cancellationToken)
//     {
//         _logger.LogInformation("Starting OCR processing for document: {DocumentId}, file: {FilePath}", 
//             message.DocumentId, message.FilePath);
//
//         try
//         {
//             // Download file from MinIO
//             await using var fileStream = await _minIoService.DownloadFileAsync(message.BucketName, message.FilePath);
//
//             // Perform OCR based on file type
//             string extractedText = Path.GetExtension(message.OriginalFileName)?.ToLower() switch
//             {
//                 ".pdf" => _ocrClient.OcrPdf(fileStream),
//                 ".jpg" or ".jpeg" or ".png" or ".tiff" or ".bmp" => _ocrClient.OcrImage(fileStream),
//                 _ => throw new NotSupportedException($"Unsupported file type: {Path.GetExtension(message.OriginalFileName)}")
//             };
//
//             // Update document in ElasticSearch with OCR text
//             var updateFields = new
//             {
//                 ocr_text = extractedText,
//                 ocr_processed = true,
//                 ocr_processed_at = DateTime.UtcNow
//             };
//
//             await _elasticSearchService.UpdateDocumentAsync(
//                 message.DocumentId, 
//                 updateFields, 
//                 cancellationToken);
//
//             _logger.LogInformation("Successfully completed OCR processing for document: {DocumentId}", message.DocumentId);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Failed to process OCR for document: {DocumentId}", message.DocumentId);
//         }
//     }
// }