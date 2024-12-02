// using Agents.Interfaces;
// using Nest;
// using PostgreSQL.Entities;
//
// namespace Agents;
//
// public class ElasticSearch : IElasticSearchServiceAgent
// {
//     private readonly ElasticClient _client;
//     private readonly string? _defaultIndex;
//
//     public ElasticSearch(string? connectionString, string? defaultIndex = "documents")
//     {
//         var settings = new ConnectionSettings(new Uri(connectionString!))
//             .DefaultIndex(defaultIndex)
//             .DefaultMappingFor<Document>(m => m
//                 .IdProperty(d => d.Id)
//                 .PropertyName(d => d.Name, "name")
//                 .PropertyName(d => d.FilePath, "file_path")
//                 .PropertyName(d => d.DateUploaded, "date_uploaded")
//             );
//
//         _client = new ElasticClient(settings);
//         _defaultIndex = defaultIndex;
//     }
//
//     //TODO:
//     public Task IndexAsync<T>(T document) where T : class
//     {
//         // Index the document in ElasticSearch 
//         return _client.IndexDocumentAsync(document);
//     }
//
//     public async Task UpdateDocumentAsync(string? documentId, object updateFields, CancellationToken cancellationToken)
//     {
//         var updateResponse = await _client.UpdateAsync<object>(documentId, u => u
//                 .Index(_defaultIndex)
//                 .Doc(updateFields)
//                 .RetryOnConflict(3), 
//             cancellationToken);
//
//         if (!updateResponse.IsValid)
//         {
//             throw new Exception($"Failed to update document {documentId}: {updateResponse.DebugInformation}");
//         }
//     }
// }