using DarkKitchen.Application.Services.ProductImports;

namespace DarkKitchen.WebApi.Responses.ProductImports;

public sealed record ProductImportIssueResponse
{
    public required string Name { get; init; }
    public required string Message { get; init; }

    public static ProductImportIssueResponse FromDto(ProductImportIssueDto dto) => new()
    {
        Name = dto.Name,
        Message = dto.Message
    };
}
