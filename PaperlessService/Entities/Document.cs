using Microsoft.AspNetCore.Http;

namespace PaperlessService.Entities;

public class BlDocument
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public DateTime DateUploaded { get; set; }
    public IFormFile? File { get; set; }
}