using DarkKitchen.Application.Services.ProductImports;

namespace DarkKitchen.WebApi.Responses.ProductImports;

public sealed record ProductImportResultResponse
{
    public required string ImporterId { get; init; }
    public required int Total { get; init; }
    public required int Created { get; init; }
    public required int SkippedDuplicates { get; init; }
    public required int Failed { get; init; }
    public required IReadOnlyList<ProductImportIssueResponse> Issues { get; init; }

    public static ProductImportResultResponse FromDto(ProductImportResultDto dto) => new()
    {
        ImporterId = dto.ImporterId,
        Total = dto.Total,
        Created = dto.Created,
        SkippedDuplicates = dto.SkippedDuplicates,
        Failed = dto.Failed,
        Issues = dto.Issues.Select(ProductImportIssueResponse.FromDto).ToList()
    };
}
