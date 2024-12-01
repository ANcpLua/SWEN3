using Microsoft.AspNetCore.Http;

namespace Contract.DTOModels;

public record DocumentUploadDto
{
    public string Title { get; init; } = default!;
    public IFormFile File { get; init; } = default!;
}