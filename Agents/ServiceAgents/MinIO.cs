// namespace Agents.ServiceAgents;
//
// public class MinIO : IMinIoServiceAgent
// {
//     private readonly IMinioClient _client;
//     private readonly ILogger<MinIo> _logger;
//
//     public MinIO(IOptions<MinIoOptions> options, ILogger<MinIO> logger)
//     {
//         _logger = logger;
//
//         var minioOptions = options.Value;
//
//         _client = new MinioClient()
//             .WithEndpoint(minioOptions.Endpoint, minioOptions.Port)
//             .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
//             .Build();
//
//         _logger.LogInformation("MinIO client successfully initialized.");
//     }
//
//     public async Task UploadFileAsync(string bucketName, string objectName, Stream fileStream)
//     {
//             if (!await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)))
//             {
//                 await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
//                 _logger.LogInformation($"Bucket {bucketName} created successfully.");
//             }
//
//             await _client.PutObjectAsync(new PutObjectArgs()
//                 .WithBucket(bucketName)
//                 .WithObject(objectName)
//                 .WithStreamData(fileStream)
//                 .WithObjectSize(fileStream.Length)
//                 .WithContentType("application/octet-stream"));
//
//             _logger.LogInformation($"Successfully uploaded object {objectName} to bucket {bucketName}.");
//     }
//
//     public async Task<Stream> DownloadFileAsync(string bucketName, string? filePath)
//     {
//             var memoryStream = new MemoryStream();
//             await _client.GetObjectAsync(new GetObjectArgs()
//                 .WithBucket(bucketName)
//                 .WithObject(filePath)
//                 .WithCallbackStream(stream => stream.CopyTo(memoryStream)));
//
//             memoryStream.Position = 0;
//             _logger.LogInformation($"Successfully downloaded file {filePath} from bucket {bucketName}.");
//             return memoryStream;
//     }
// }