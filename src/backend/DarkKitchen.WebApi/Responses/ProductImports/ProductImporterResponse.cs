using DarkKitchen.Application.Services.ProductImports;

namespace DarkKitchen.WebApi.Responses.ProductImports;

public sealed record ProductImporterResponse
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }

    public static ProductImporterResponse FromDto(ProductImporterReadDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description
    };
}
