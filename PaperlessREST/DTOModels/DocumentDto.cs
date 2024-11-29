namespace PaperlessREST.DTOModels;

// For API responses (GET)
public record DocumentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public DateTime DateUploaded { get; set; }
}
