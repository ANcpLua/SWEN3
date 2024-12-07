using PostgreSQL.Entities;

namespace PostgreSQL.Persistence
{
    public interface IDocumentRepository
    {
        Task<Document?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Document>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Document> Upload(Document document, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task DeleteAllAsync(CancellationToken cancellationToken = default);
    }
}
